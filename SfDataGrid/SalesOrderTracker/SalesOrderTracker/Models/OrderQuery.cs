using System;

namespace SalesOrderTracker.Models
{
    public class OrderQuery
    {
        public OrderStatus? Status { get; set; }
        public string? CustomerName { get; set; }
        public string? OrderNumber { get; set; }
    }
}
