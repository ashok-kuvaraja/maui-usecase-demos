using System;
using System.Threading.Tasks;
using SalesOrderTracker.Models;

namespace SalesOrderTracker.Contracts
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid id);
        Task<OrderListItemDto[]> QueryAsync(OrderQuery query, int limit = 100, int offset = 0);
        Task InsertAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(Guid id);
        Task UpdateStatusAsync(Guid id, OrderStatus newStatus, string? changedBy = null);
        Task SeedSampleDataAsync();
    }
}
