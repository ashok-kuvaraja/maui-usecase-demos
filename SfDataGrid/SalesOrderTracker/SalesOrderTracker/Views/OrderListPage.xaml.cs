using System;
using Microsoft.Maui.Controls;
using SalesOrderTracker.ViewModels;
using SalesOrderTracker.Models;
using Microsoft.Maui.Graphics;
using SalesOrderTracker.Contracts;
using Syncfusion.Maui.DataGrid;

namespace SalesOrderTracker.Views
{
    public partial class OrderListPage : ContentPage
    {
        private readonly OrderListViewModel _vm;

        // Parameterless ctor used by Shell DataTemplate — resolves ViewModel from DI
        public OrderListPage() : this(
            App.Services!.GetService(typeof(ViewModels.OrderListViewModel)) as ViewModels.OrderListViewModel
            ?? new ViewModels.OrderListViewModel(
                App.Services!.GetService(typeof(IOrderRepository)) as IOrderRepository
                ?? throw new InvalidOperationException("IOrderRepository not registered")))
        { }

        public OrderListPage(OrderListViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            BindingContext = _vm;

            // Build columns for SfDataGrid
            OrdersGrid.Columns.Clear();
            OrdersGrid.Columns.Add(new DataGridTextColumn { MappingName = nameof(OrderListItemDto.OrderNumber), HeaderText = "Order #" });
            OrdersGrid.Columns.Add(new DataGridTextColumn { MappingName = nameof(OrderListItemDto.CustomerName), HeaderText = "Customer" });
            OrdersGrid.Columns.Add(new DataGridTextColumn { MappingName = nameof(OrderListItemDto.OrderDate), HeaderText = "Date" });

            // Status column with colored badge based on status
            var statusColumn = new DataGridTemplateColumn { MappingName = nameof(OrderListItemDto.Status), HeaderText = "Status" };
            statusColumn.CellTemplate = new DataTemplate(() =>
            {
                var lbl = new Label
                {
                    FontSize = 12,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Padding = new Thickness(6),
                    BackgroundColor = Colors.Transparent
                };

                lbl.SetBinding(Label.TextProperty, new Binding(nameof(OrderListItemDto.Status)));

                // Color badges for statuses
                lbl.Triggers.Add(new DataTrigger(typeof(Label))
                {
                    Binding = new Binding(nameof(OrderListItemDto.Status)),
                    Value = OrderStatus.Pending.ToString(),
                    Setters =
                    {
                        new Setter { Property = Label.BackgroundColorProperty, Value = Color.FromArgb("#F0AD4E") },
                        new Setter { Property = Label.TextColorProperty, Value = Colors.Black }
                    }
                });

                lbl.Triggers.Add(new DataTrigger(typeof(Label))
                {
                    Binding = new Binding(nameof(OrderListItemDto.Status)),
                    Value = OrderStatus.Processing.ToString(),
                    Setters =
                    {
                        new Setter { Property = Label.BackgroundColorProperty, Value = Color.FromArgb("#6FB3F2") },
                        new Setter { Property = Label.TextColorProperty, Value = Colors.Black }
                    }
                });

                lbl.Triggers.Add(new DataTrigger(typeof(Label))
                {
                    Binding = new Binding(nameof(OrderListItemDto.Status)),
                    Value = OrderStatus.Shipped.ToString(),
                    Setters =
                    {
                        new Setter { Property = Label.BackgroundColorProperty, Value = Color.FromArgb("#FFB74D") },
                        new Setter { Property = Label.TextColorProperty, Value = Colors.Black }
                    }
                });

                lbl.Triggers.Add(new DataTrigger(typeof(Label))
                {
                    Binding = new Binding(nameof(OrderListItemDto.Status)),
                    Value = OrderStatus.Delivered.ToString(),
                    Setters =
                    {
                        new Setter { Property = Label.BackgroundColorProperty, Value = Color.FromArgb("#66BB6A") },
                        new Setter { Property = Label.TextColorProperty, Value = Colors.White }
                    }
                });

                lbl.Triggers.Add(new DataTrigger(typeof(Label))
                {
                    Binding = new Binding(nameof(OrderListItemDto.Status)),
                    Value = OrderStatus.Cancelled.ToString(),
                    Setters =
                    {
                        new Setter { Property = Label.BackgroundColorProperty, Value = Color.FromArgb("#E57373") },
                        new Setter { Property = Label.TextColorProperty, Value = Colors.White }
                    }
                });

                return lbl;
            });
            OrdersGrid.Columns.Add(statusColumn);

            // Action column with inline toggle button bound to UpdateStatusCommand on the page's ViewModel
            var template = new DataGridTemplateColumn { HeaderText = "Actions" };
            template.CellTemplate = new DataTemplate(() =>
            {
                var btn = new Button { Text = "Toggle Status", FontSize = 12 };
                btn.SetBinding(Button.CommandProperty, new Binding("BindingContext.UpdateStatusCommand", source: this));
                btn.SetBinding(Button.CommandParameterProperty, new Binding("."));
                return btn;
            });
            OrdersGrid.Columns.Add(template);
            // Open order details when a row is tapped
            OrdersGrid.CellTapped += OrdersGrid_CellTapped;

            
        }

        private void OrdersGrid_CellTapped(object? sender, DataGridCellTappedEventArgs e)
        {
            try
            {
                if (e.RowData is OrderListItemDto item)
                {
                    if (_vm.OpenOrderCommand.CanExecute(item))
                        _vm.OpenOrderCommand.Execute(item);
                }
            }
            catch
            {
                // swallow navigation errors from UI events
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _vm.LoadAsync();

            // Populate status filter picker
            StatusPicker.ItemsSource = System.Enum.GetNames(typeof(OrderStatus));
        }

        private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            this.OrdersGrid.SearchController.Search(SearchBox.Text);
        }

        private async void OnStatusFilterChanged(object sender, EventArgs e)
        {
            if (StatusPicker.SelectedIndex >= 0 && StatusPicker.SelectedItem is string status)
            {
                if (System.Enum.TryParse<OrderStatus>(status, out var orderStatus))
                {
                    var query = new OrderQuery { Status = orderStatus };
                    await _vm.ApplyFilterAsync(query);
                }
            }
            else if (StatusPicker.SelectedIndex == -1 || StatusPicker.SelectedItem == null)
            {
                // Reset to all
                await _vm.LoadAsync();
            }
        }

        // Optional: handle search icon click to focus the entry on platforms where SearchBar behaves oddly
        private void OnSearchIconClicked(object sender, EventArgs e)
        {
            try
            {
                SearchBox?.Focus();
            }
            catch { }
        }
    }
}
