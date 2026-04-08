using System;
using SQLite;

namespace SalesOrderTracker.Models
{
    public class LineItem
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        [Indexed]
        public Guid OrderId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal => Quantity * UnitPrice;
    }
}
