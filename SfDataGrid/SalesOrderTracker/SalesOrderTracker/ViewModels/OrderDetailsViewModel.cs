using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SalesOrderTracker.Contracts;
using SalesOrderTracker.Models;

namespace SalesOrderTracker.ViewModels
{
    public partial class OrderDetailsViewModel : ObservableObject
    {
        private readonly IOrderRepository _repo;
        private readonly SalesOrderTracker.Services.Storage.Database _db;

        public Order? CurrentOrder { get; private set; }
        public string CustomerName { get; private set; } = string.Empty;

        public System.Windows.Input.ICommand MarkProcessingCommand { get; }
        public System.Windows.Input.ICommand MarkShippedCommand { get; }
        public System.Windows.Input.ICommand MarkDeliveredCommand { get; }
        public System.Windows.Input.ICommand CancelOrderCommand { get; }

        public OrderDetailsViewModel(IOrderRepository repo, SalesOrderTracker.Services.Storage.Database db)
        {
            _repo = repo;
            _db = db;

            MarkProcessingCommand = new Microsoft.Maui.Controls.Command(async () =>
            {
                if (CurrentOrder != null)
                    await UpdateStatusAsync(CurrentOrder.Id, OrderStatus.Processing);
            });

            MarkShippedCommand = new Microsoft.Maui.Controls.Command(async () =>
            {
                if (CurrentOrder != null)
                    await UpdateStatusAsync(CurrentOrder.Id, OrderStatus.Shipped);
            });

            MarkDeliveredCommand = new Microsoft.Maui.Controls.Command(async () =>
            {
                if (CurrentOrder != null)
                    await UpdateStatusAsync(CurrentOrder.Id, OrderStatus.Delivered);
            });

            CancelOrderCommand = new Microsoft.Maui.Controls.Command(async () =>
            {
                if (CurrentOrder != null)
                    await UpdateStatusAsync(CurrentOrder.Id, OrderStatus.Cancelled);
            });
        }

        public async Task LoadAsync(Guid id)
        {
            CurrentOrder = await _repo.GetByIdAsync(id);
            // lookup customer name
            try
            {
                var cust = await _db.Connection.FindAsync<SalesOrderTracker.Models.Customer>(CurrentOrder.CustomerId);
                CustomerName = cust?.CustomerName ?? CurrentOrder.CustomerId.ToString();
            }
            catch
            {
                CustomerName = CurrentOrder.CustomerId.ToString();
            }

            OnPropertyChanged(nameof(CurrentOrder));
            OnPropertyChanged(nameof(CustomerName));
        }

        public async Task UpdateStatusAsync(Guid id, OrderStatus newStatus)
        {
            await _repo.UpdateStatusAsync(id, newStatus);
            await LoadAsync(id);
        }
    }
}
