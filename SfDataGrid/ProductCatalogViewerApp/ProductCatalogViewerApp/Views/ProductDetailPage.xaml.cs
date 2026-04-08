using ProductCatalogViewerApp.Models;
using ProductCatalogViewerApp.ViewModels;

namespace ProductCatalogViewerApp.Views
{
    /// <summary>
    /// Code-behind for Product Detail Page.
    /// Receives the selected product via Shell navigation query parameter.
    /// </summary>
    [QueryProperty(nameof(Product), "Product")]
    public partial class ProductDetailPage : ContentPage
    {
        private ProductDetailViewModel ViewModel => (ProductDetailViewModel)BindingContext;

        /// <summary>
        /// Product set via Shell navigation query parameter "Product".
        /// </summary>
        public Product? Product
        {
            set
            {
                if (BindingContext is ProductDetailViewModel vm)
                {
                    vm.Product = value;
                    LoadProductImage(value?.Image);
                }
            }
        }

        public ProductDetailPage(ProductDetailViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        /// <summary>
        /// Loads the product image, falling back to the placeholder on error.
        /// </summary>
        private void LoadProductImage(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                ProductImage.Source = "dotnet_bot.png";
                return;
            }

            ProductImage.Source = imageUrl;
        }
    }
}
