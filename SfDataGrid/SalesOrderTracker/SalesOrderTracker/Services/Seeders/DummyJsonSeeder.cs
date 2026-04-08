using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SalesOrderTracker.Contracts;
using SalesOrderTracker.Models;
using SalesOrderTracker.Services.Storage;
using System.Linq;
using System.Collections.Generic;

namespace SalesOrderTracker.Services.Seeders
{
    internal static class DummyJsonSeeder
    {
        private static readonly HttpClient _client = new HttpClient();

        public static async Task SeedFromDummyJsonAsync(IServiceProvider services, int limit = 20)
        {
            using var scope = services.CreateScope();
            var svc = scope.ServiceProvider;
            var db = svc.GetRequiredService<Database>();
            var logger = svc.GetService<ILoggerFactory>()?.CreateLogger("DummyJsonSeeder");

            try
            {
                var any = await db.Connection.Table<Order>().FirstOrDefaultAsync();
                if (any != null) return; // nothing to do

                var repo = svc.GetService<IOrderRepository>();
                if (repo == null) return;

                var url = $"https://dummyjson.com/carts?limit={limit}";
                var resp = await _client.GetAsync(url);
                if (!resp.IsSuccessStatusCode) return;
                using var stream = await resp.Content.ReadAsStreamAsync();
                var doc = await JsonDocument.ParseAsync(stream);
                if (!doc.RootElement.TryGetProperty("carts", out var carts)) return;

                var userMap = new Dictionary<int, Guid>();

                foreach (var cart in carts.EnumerateArray())
                {
                    try
                    {
                        var cartId = cart.GetProperty("id").GetInt32();
                        var userId = cart.GetProperty("userId").GetInt32();
                        var total = cart.GetProperty("total").GetDecimal();
                        var order = new Order
                        {
                            Id = Guid.NewGuid(),
                            OrderNumber = $"DJ-{cartId}",
                            OrderDate = DateTime.UtcNow,
                            Status = OrderStatus.Pending,
                            TotalAmount = total,
                            LineItems = new List<LineItem>(),
                            StatusHistory = new List<OrderStatusEntry>()
                        };

                        // ensure customer
                        if (!userMap.TryGetValue(userId, out var custGuid))
                        {
                            custGuid = Guid.NewGuid();
                            userMap[userId] = custGuid;
                            // insert a customer record
                            await db.Connection.InsertAsync(new Customer
                            {
                                Id = custGuid,
                                CustomerName = $"DummyUser {userId}"
                            });
                        }
                        order.CustomerId = userMap[userId];

                        if (cart.TryGetProperty("products", out var products))
                        {
                            foreach (var p in products.EnumerateArray())
                            {
                                var title = p.GetProperty("title").GetString() ?? "Item";
                                var price = p.GetProperty("price").GetDecimal();
                                var qty = p.GetProperty("quantity").GetInt32();
                                var li = new LineItem
                                {
                                    Id = Guid.NewGuid(),
                                    ProductName = title,
                                    Quantity = qty,
                                    UnitPrice = price
                                };
                                order.LineItems.Add(li);
                            }
                        }

                        // add initial history
                        order.StatusHistory.Add(new OrderStatusEntry
                        {
                            Id = Guid.NewGuid(),
                            OrderId = order.Id,
                            OldStatus = order.Status,
                            NewStatus = order.Status,
                            ChangedAt = order.OrderDate
                        });

                        // compute total if missing
                        if (order.TotalAmount <= 0 && order.LineItems != null)
                        {
                            order.TotalAmount = order.LineItems.Sum(x => x.UnitPrice * x.Quantity);
                        }

                        await repo.InsertAsync(order);
                    }
                    catch (Exception ex)
                    {
                        logger?.LogDebug(ex, "Skipping cart seeding item");
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogWarning(ex, "DummyJSON seeder failed");
            }
        }
    }
}
