using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SalesOrderTracker.Contracts;
using SalesOrderTracker.Models;
using Microsoft.Maui.Controls;

namespace SalesOrderTracker.ViewModels
{
    public partial class OrderListViewModel : ObservableObject
    {
        private readonly IOrderRepository _repo;

        public ObservableCollection<OrderListItemDto> Orders { get; } = new();

        public System.Windows.Input.ICommand OpenOrderCommand { get; }
        public System.Windows.Input.ICommand UpdateStatusCommand { get; }

        public OrderListViewModel(IOrderRepository repo)
        {
            _repo = repo;
            OpenOrderCommand = new Microsoft.Maui.Controls.Command<OrderListItemDto>(async item =>
            {
                if (item == null) return;
                await Shell.Current.GoToAsync($"/OrderDetailsPage?orderId={item.Id}");
            });

            UpdateStatusCommand = new Microsoft.Maui.Controls.Command<OrderListItemDto>(async item =>
            {
                if (item == null) return;
                var newStatus = GetNextStatus(item.Status);
                if (newStatus == item.Status) return; // nothing to do
                await _repo.UpdateStatusAsync(item.Id, newStatus);
                await LoadAsync();
            });
        }

        private OrderStatus GetNextStatus(OrderStatus current)
        {
            // Advance through the lifecycle until Delivered. Do not advance Cancelled.
            if (current == OrderStatus.Cancelled || current == OrderStatus.Delivered)
                return current;

            // Safe cast to int to move to next enum value
            var next = (OrderStatus)(((int)current) + 1);
            return next;
        }

        public async Task LoadAsync()
        {
            var results = await _repo.QueryAsync(new OrderQuery());
            Orders.Clear();
            foreach (var r in results)
                Orders.Add(r);
        }

        public async Task ApplyFilterAsync(OrderQuery query)
        {
            var results = await _repo.QueryAsync(query);
            Orders.Clear();
            foreach (var r in results)
                Orders.Add(r);
        }
        
    }
}
