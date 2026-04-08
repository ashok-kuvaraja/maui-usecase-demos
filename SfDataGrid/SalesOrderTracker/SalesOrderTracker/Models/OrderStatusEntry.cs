using System;
using SQLite;

namespace SalesOrderTracker.Models
{
    public class OrderStatusEntry
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        [Indexed]
        public Guid OrderId { get; set; }
        public OrderStatus OldStatus { get; set; }
        public OrderStatus NewStatus { get; set; }
        public string? ChangedBy { get; set; }
        public DateTime ChangedAt { get; set; }
        public string? Notes { get; set; }
    }
}
