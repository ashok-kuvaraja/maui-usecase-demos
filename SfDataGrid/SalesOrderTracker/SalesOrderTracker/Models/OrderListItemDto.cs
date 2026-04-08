using System;

namespace SalesOrderTracker.Models
{
    public class OrderListItemDto
    {
        public Guid Id { get; set; }
        public string? OrderNumber { get; set; }
        public string? CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
