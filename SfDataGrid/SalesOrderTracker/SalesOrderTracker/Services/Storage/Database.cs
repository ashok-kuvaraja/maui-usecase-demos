using System;
using System.Threading.Tasks;
using SQLite;
using SalesOrderTracker.Models;

namespace SalesOrderTracker.Services.Storage
{
    public class Database
    {
        private readonly SQLiteAsyncConnection _conn;

        public Database(string path)
        {
            _conn = new SQLiteAsyncConnection(path);
        }

        public SQLiteAsyncConnection Connection => _conn;

        public async Task InitAsync()
        {
            await _conn.CreateTableAsync<Order>();
            await _conn.CreateTableAsync<Customer>();
            await _conn.CreateTableAsync<LineItem>();
            await _conn.CreateTableAsync<OrderStatusEntry>();
        }
    }
}
