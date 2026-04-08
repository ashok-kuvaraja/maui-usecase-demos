using System;
using Microsoft.Maui.Controls;
using SalesOrderTracker.ViewModels;
using SalesOrderTracker.Models;

namespace SalesOrderTracker.Views
{
    [QueryProperty("OrderId", "orderId")]
    public partial class OrderDetailsPage : ContentPage
    {
        private readonly OrderDetailsViewModel _vm;
        private Guid _orderId;

        public string OrderId
        {
            set
            {
                if (Guid.TryParse(value, out var id))
                {
                    _orderId = id;
                    _ = _vm.LoadAsync(id);
                }
            }
        }

        // Parameterless ctor used by Shell route navigation
        public OrderDetailsPage() : this(
            App.Services!.GetService(typeof(OrderDetailsViewModel)) as OrderDetailsViewModel
            ?? new OrderDetailsViewModel(
                App.Services!.GetService(typeof(SalesOrderTracker.Contracts.IOrderRepository)) as SalesOrderTracker.Contracts.IOrderRepository
                ?? throw new InvalidOperationException("IOrderRepository not registered"),
                App.Services!.GetService(typeof(SalesOrderTracker.Services.Storage.Database)) as SalesOrderTracker.Services.Storage.Database
                ?? throw new InvalidOperationException("Database not registered")))
        { }

        public OrderDetailsPage(OrderDetailsViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            BindingContext = _vm;
        }

        private async void OnMarkProcessingClicked(object sender, EventArgs e)
        {
            if (_orderId == Guid.Empty) return;
            await _vm.UpdateStatusAsync(_orderId, OrderStatus.Processing);
        }
    }
}
