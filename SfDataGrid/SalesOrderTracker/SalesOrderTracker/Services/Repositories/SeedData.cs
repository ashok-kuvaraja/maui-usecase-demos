using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SalesOrderTracker.Contracts;
using SalesOrderTracker.Services.Seeders;
using SalesOrderTracker.Services.Storage;
using SalesOrderTracker.Models;
using Microsoft.Maui.Storage;

namespace SalesOrderTracker.Services.Repositories
{
    public static class SeedData
    {
        /// <summary>
        /// Fully async seed method — safe to call from a background Task.
        /// First tries local Bogus fake data; if still empty, falls back to DummyJSON /carts.
        /// </summary>
        public static async Task EnsureSeedAsync(IServiceProvider services, int limit = 50, bool force = true)
        {
            try
            {
                // Check preference for data source
                var useDummy = Preferences.Default.Get("UseDummyJson", true);

                using var scope = services.CreateScope();
                var svc = scope.ServiceProvider;
                var repo = svc.GetService<IOrderRepository>();
                var db = svc.GetService<Database>();

                if (useDummy)
                {
                    try
                    {
                        await DummyJsonSeeder.SeedFromDummyJsonAsync(services, limit);
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        if (force && db != null)
                        {
                            try
                            {
                                await db.Connection.ExecuteAsync("DELETE FROM LineItem");
                                await db.Connection.ExecuteAsync("DELETE FROM OrderStatusEntry");
                                await db.Connection.ExecuteAsync("DELETE FROM [Order]");
                                await db.Connection.ExecuteAsync("DELETE FROM Customer");
                            }
                            catch { }
                        }

                        if (repo != null)
                        {
                            await repo.SeedSampleDataAsync();
                        }
                    }
                    catch { }
                }
            }
            catch
            {
                // swallow seed errors at startup
            }
        }

    }
}
