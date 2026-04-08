using System;
using SQLite;

namespace SalesOrderTracker.Models
{
    public class Customer
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public string? CustomerName { get; set; }
        public string? ContactInfo { get; set; }
    }
}
