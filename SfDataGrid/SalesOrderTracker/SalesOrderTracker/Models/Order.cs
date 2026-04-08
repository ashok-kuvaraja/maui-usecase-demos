using System;
using System.Collections.Generic;
using SQLite;

namespace SalesOrderTracker.Models
{
    public class Order
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public string? OrderNumber { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        [Ignore]
        public List<LineItem>? LineItems { get; set; }
        [Ignore]
        public List<OrderStatusEntry>? StatusHistory { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
