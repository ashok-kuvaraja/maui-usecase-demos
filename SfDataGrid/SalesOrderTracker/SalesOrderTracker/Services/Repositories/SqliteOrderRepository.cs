using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using SalesOrderTracker.Contracts;
using SalesOrderTracker.Models;
using SalesOrderTracker.Services.Storage;
using SQLite;
using Bogus;

namespace SalesOrderTracker.Services.Repositories
{
    public class SqliteOrderRepository : IOrderRepository
    {
        private readonly SalesOrderTracker.Services.Storage.Database _database;
        private readonly Microsoft.Extensions.Logging.ILogger<SqliteOrderRepository> _logger;

        public SqliteOrderRepository(SalesOrderTracker.Services.Storage.Database database, Microsoft.Extensions.Logging.ILogger<SqliteOrderRepository> logger)
        {
            _database = database;
            _logger = logger;
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var db = _database.Connection;
                await db.DeleteAsync<Order>(id);
                await db.ExecuteAsync("DELETE FROM LineItem WHERE OrderId = ?", id.ToString());
                await db.ExecuteAsync("DELETE FROM OrderStatusEntry WHERE OrderId = ?", id.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting order {OrderId}", id);
                throw new RepositoryException("Failed to delete order", ex);
            }
        }

        public async Task<Order?> GetByIdAsync(Guid id)
        {
            try
            {
                var db = _database.Connection;
                var order = await db.FindAsync<Order>(id);
                if (order == null) return null;
                var items = await db.Table<LineItem>().Where(li => li.OrderId == id).ToListAsync();
                var history = await db.Table<OrderStatusEntry>().Where(h => h.OrderId == id).OrderBy(h => h.ChangedAt).ToListAsync();
                order.LineItems = items;
                order.StatusHistory = history;
                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading order {OrderId}", id);
                throw new RepositoryException("Failed to load order", ex);
            }
        }

        public async Task<OrderListItemDto[]> QueryAsync(OrderQuery query, int limit = 100, int offset = 0)
        {
            try
            {
                var db = _database.Connection;
                var orders = await db.Table<Order>().OrderByDescending(o => o.OrderDate).Skip(offset).Take(limit).ToListAsync();
                // Load customers for name mapping
                var customers = (await db.Table<Customer>().ToListAsync()).ToDictionary(c => c.Id, c => c.CustomerName);
                var filtered = orders.AsEnumerable();
                if (query != null)
                {
                    if (query.Status.HasValue)
                        filtered = filtered.Where(o => o.Status == query.Status.Value);
                    if (!string.IsNullOrWhiteSpace(query.OrderNumber))
                        filtered = filtered.Where(o => (o.OrderNumber ?? string.Empty).Contains(query.OrderNumber!, StringComparison.OrdinalIgnoreCase));
                    if (!string.IsNullOrWhiteSpace(query.CustomerName))
                        filtered = filtered.Where(o => customers.TryGetValue(o.CustomerId, out var name) && (name ?? string.Empty).Contains(query.CustomerName!, StringComparison.OrdinalIgnoreCase));
                }

                var result = filtered.Select(o => new OrderListItemDto
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    CustomerName = customers.TryGetValue(o.CustomerId, out var cn) ? cn : string.Empty,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    TotalAmount = o.TotalAmount
                }).ToArray();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying orders");
                throw new RepositoryException("Failed to query orders", ex);
            }
        }

        public async Task InsertAsync(Order order)
        {
            try
            {
                var db = _database.Connection;
                order.CreatedAt = DateTime.UtcNow;
                order.UpdatedAt = order.CreatedAt;
                if (order.Id == Guid.Empty) order.Id = Guid.NewGuid();
                await db.InsertAsync(order);
                if (order.LineItems != null)
                {
                    foreach (var li in order.LineItems)
                    {
                        if (li.Id == Guid.Empty) li.Id = Guid.NewGuid();
                        li.OrderId = order.Id;
                        await db.InsertAsync(li);
                    }
                }
                if (order.StatusHistory != null)
                {
                    foreach (var h in order.StatusHistory)
                    {
                        if (h.Id == Guid.Empty) h.Id = Guid.NewGuid();
                        h.OrderId = order.Id;
                        await db.InsertAsync(h);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting order {OrderNumber}", order.OrderNumber);
                throw new RepositoryException("Failed to insert order", ex);
            }
        }

        public async Task UpdateAsync(Order order)
        {
            try
            {
                var db = _database.Connection;
                order.UpdatedAt = DateTime.UtcNow;
                await db.UpdateAsync(order);
                // Replace line items for simplicity
                await db.ExecuteAsync("DELETE FROM LineItem WHERE OrderId = ?", order.Id.ToString());
                if (order.LineItems != null)
                {
                    foreach (var li in order.LineItems)
                    {
                        if (li.Id == Guid.Empty) li.Id = Guid.NewGuid();
                        li.OrderId = order.Id;
                        await db.InsertAsync(li);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order {OrderId}", order.Id);
                throw new RepositoryException("Failed to update order", ex);
            }
        }

        public async Task UpdateStatusAsync(Guid id, OrderStatus newStatus, string? changedBy = null)
        {
            try
            {
                var db = _database.Connection;
                var order = await db.FindAsync<Order>(id);
                if (order == null) throw new RepositoryException($"Order {id} not found");
                var entry = new OrderStatusEntry
                {
                    Id = Guid.NewGuid(),
                    OrderId = id,
                    OldStatus = order.Status,
                    NewStatus = newStatus,
                    ChangedBy = changedBy,
                    ChangedAt = DateTime.UtcNow
                };
                order.Status = newStatus;
                order.UpdatedAt = DateTime.UtcNow;
                await db.InsertAsync(entry);
                await db.UpdateAsync(order);
            }
            catch (RepositoryException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for order {OrderId}", id);
                throw new RepositoryException("Failed to update status", ex);
            }
        }

        public async Task SeedSampleDataAsync()
        {
            try
            {
                var db = _database.Connection;
                var existing = await db.Table<Order>().FirstOrDefaultAsync();
                if (existing != null) return;

                // Use Bogus to generate realistic fake data for local development
                var rnd = new Random();

                var customerFaker = new Faker<Customer>()
                    .RuleFor(c => c.Id, f => Guid.NewGuid())
                    .RuleFor(c => c.CustomerName, f => f.Company.CompanyName());

                var lineItemFaker = new Faker<LineItem>()
                    .RuleFor(li => li.Id, f => Guid.NewGuid())
                    .RuleFor(li => li.ProductName, f => f.Commerce.ProductName())
                    .RuleFor(li => li.Quantity, f => f.Random.Int(1, 5))
                    .RuleFor(li => li.UnitPrice, f => Math.Round(f.Random.Decimal(5m, 500m), 2));

                var orderFaker = new Faker<Order>()
                    .RuleFor(o => o.Id, f => Guid.NewGuid())
                    .RuleFor(o => o.OrderNumber, f => $"SO-{f.Random.Number(1000, 9999)}")
                    .RuleFor(o => o.OrderDate, f => f.Date.Recent(30))
                    .RuleFor(o => o.Status, f => f.PickRandom<OrderStatus>())
                    .RuleFor(o => o.CreatedAt, (f, o) => o.OrderDate)
                    .RuleFor(o => o.UpdatedAt, (f, o) => o.OrderDate);

                // create a small catalog of customers
                var customers = customerFaker.Generate(10);
                foreach (var c in customers)
                {
                    await db.InsertAsync(c);
                }

                var ordersToCreate = 40;
                for (int i = 0; i < ordersToCreate; i++)
                {
                    var order = orderFaker.Generate();
                    // pick a random customer
                    order.CustomerId = customers[rnd.Next(customers.Count)].Id;

                    // generate 1-4 line items
                    var items = lineItemFaker.Generate(rnd.Next(1, 5));
                    foreach (var it in items)
                    {
                        it.OrderId = order.Id;
                    }
                    order.LineItems = items;

                    // compute total
                    order.TotalAmount = items.Sum(x => x.UnitPrice * x.Quantity);

                    // add an initial status history entry
                    var history = new OrderStatusEntry
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        OldStatus = order.Status,
                        NewStatus = order.Status,
                        ChangedAt = order.OrderDate
                    };
                    order.StatusHistory = new List<OrderStatusEntry> { history };

                    await InsertAsync(order);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding sample data");
                throw new RepositoryException("Failed to seed sample data", ex);
            }
        }
    }
}
