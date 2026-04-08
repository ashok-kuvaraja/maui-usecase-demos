using ProductCatalogViewerApp.Models;
using ProductCatalogViewerApp.ViewModels;
using Syncfusion.Maui.DataGrid;
using Syncfusion.Maui.Themes;

namespace ProductCatalogViewerApp.Views
{
    /// <summary>
    /// Code-behind for Product Catalog Page.
    /// Connects UI events to the ProductCatalogViewModel.
    /// </summary>
    public partial class ProductCatalogPage : ContentPage
    {
        private ProductCatalogViewModel ViewModel => (ProductCatalogViewModel)BindingContext;

        public ProductCatalogPage(ProductCatalogViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        // ─── Page lifecycle ────────────────────────────────────────────────

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Load products on first appearance
            await ViewModel.LoadProductsAsync();

            // Initialize DataGrid search settings
            ProductDataGrid.SearchController.AllowFiltering = true;
            ProductDataGrid.SearchController.SearchType = Syncfusion.Maui.DataGrid.DataGridSearchType.Contains;
            ProductDataGrid.SearchController.AllowCaseSensitive = false;

            // Update empty / error state display
            UpdateEmptyAndErrorState();
        }

        // ─── Search (DataGrid Search Controller) ──────────────────────────

        private void OnSearchEntryTextChanged(object? sender, TextChangedEventArgs e)
        {
            var searchText = e.NewTextValue ?? string.Empty;

            // Update ViewModel search term to trigger FilteredProducts update
            // This ensures the empty state logic works correctly
            ViewModel.UpdateSearch(searchText);

            // Also use DataGrid SearchController for highlighting and filtering
            ProductDataGrid.SearchController.AllowFiltering = true;
            ProductDataGrid.SearchController.Search(searchText);

            // Clear search results if search is empty
            if (string.IsNullOrEmpty(searchText))
            {
                ProductDataGrid.SearchController.ClearSearch();
                ProductDataGrid.Refresh();
            }

            UpdateEmptyAndErrorState();
        }

        // ─── Category filter (SfPicker) ───────────────────────────────────

        /// <summary>
        /// Handles SfPicker SelectedIndexChanged event for category filtering.
        /// Gets the selected category and updates the ViewModel filter.
        /// </summary>
        private void OnCategoryPickerSelectedIndexChanged(object? sender, Syncfusion.Maui.Picker.PickerSelectionChangedEventArgs e)
        {
            if (sender is Syncfusion.Maui.Picker.SfPicker picker &&
                picker.Columns?.Count > 0 &&
                picker.Columns[0].SelectedIndex >= 0)
                {
                    // Get the selected index from the first column
                    int selectedIndex = picker.Columns[0].SelectedIndex;

                    // Get the selected item from the column's ItemsSource
                    if (picker.Columns[0].ItemsSource is System.Collections.IList itemsSource &&
                        selectedIndex >= 0 &&
                        selectedIndex < itemsSource.Count)
                    {
                        var selectedItem = itemsSource[selectedIndex] as string ?? "All";
                        ViewModel.FilterByCategory(selectedItem);
                    }
                }
        }

        // ─── Row selection / navigation ────────────────────────────────────

        private async void OnDataGridSelectionChanged(object? sender, DataGridSelectionChangedEventArgs e)
        {
            if (e.AddedRows?.Count > 0 && e.AddedRows[0] is Product selectedProduct)
            {
                // Clear selection for clean UX after navigation
                ProductDataGrid.SelectedRows?.Clear();

                // Navigate to details page with product ID
                await Shell.Current.GoToAsync($"details?id={selectedProduct.Id}",
                    new Dictionary<string, object>
                    {
                        { "Product", selectedProduct }
                    });
            }
        }

        // ─── Empty / Error state management ───────────────────────────────

        private void UpdateEmptyAndErrorState()
        {
            var vm = ViewModel;

            // Show error frame if there's an error message
            var hasError = !string.IsNullOrEmpty(vm.ErrorMessage);
            ErrorFrame.IsVisible = hasError;
            if (hasError)
                ErrorLabel.Text = vm.ErrorMessage;

            // Show empty state when not loading, no error, and no products
            var isEmpty = !vm.IsLoading && !hasError && vm.FilteredProducts.Count == 0;
            EmptyStateLayout.IsVisible = isEmpty;

            if (isEmpty)
            {
                EmptyStateLabel.Text = string.IsNullOrWhiteSpace(vm.SearchTerm) && vm.SelectedCategory == "All"
                    ? "No products available."
                    : "No products match your search.";
            }

            // Show DataGrid only when we have data
            ProductDataGrid.IsVisible = !vm.IsLoading && !isEmpty && !hasError;
        }

        // Listen to ViewModel property changes to update UI state
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext is ProductCatalogViewModel vm)
            {
                vm.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName is nameof(vm.IsLoading)
                        or nameof(vm.ErrorMessage)
                        or nameof(vm.FilteredProducts))
                    {
                        MainThread.BeginInvokeOnMainThread(UpdateEmptyAndErrorState);
                    }
                };
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            this.CategoryPicker.IsOpen = true;
        }

        private void OnThemeIconTapped(object sender, TappedEventArgs e)
        {

            var current = Application.Current.RequestedTheme;
            var next = current == AppTheme.Dark ? AppTheme.Light : AppTheme.Dark;

            // 1) Toggle MAUI theme (this makes your AppThemeBinding/DynamicResource values react)
            Application.Current.UserAppTheme = next;

            // 2) Keep Syncfusion controls in sync
            var sfTheme = Application.Current.Resources.MergedDictionaries
                            .OfType<SyncfusionThemeResourceDictionary>()
                            .FirstOrDefault();
            if (sfTheme is not null)
            {
                sfTheme.VisualTheme = next == AppTheme.Dark
                    ? SfVisuals.MaterialDark
                    : SfVisuals.MaterialLight;
            }
        }

        private void ProductDataGrid_QueryRowHeight(object sender, DataGridQueryRowHeightEventArgs e)
        {
            if (e.RowIndex != 0)
            {
                //Calculates and sets the height of the row based on its content.
                e.Height = e.GetIntrinsicRowHeight(e.RowIndex);
                e.Handled = true;
            }
        }
    }
}
