using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProductCatalogViewerApp.Models;

namespace ProductCatalogViewerApp.ViewModels
{
    /// <summary>
    /// ViewModel for the Product Details page.
    /// Holds the selected product and provides back navigation.
    /// </summary>
    public partial class ProductDetailViewModel : ObservableObject
    {
        [ObservableProperty]
        private Product? _product;

        /// <summary>Formatted price string (e.g., "$99.99").</summary>
        public string FormattedPrice => Product is not null
            ? $"${Product.Price:F2}"
            : string.Empty;

        /// <summary>Formatted rating string (e.g., "★ 4.5 / 5 (120 reviews)").</summary>
        public string FormattedRating
        {
            get
            {
                if (Product?.Rating is null)
                    return "No rating available";

                var count = Product.RatingCount.HasValue
                    ? $" ({Product.RatingCount} reviews)"
                    : string.Empty;

                return $"★ {Product.Rating:F1} / 5{count}";
            }
        }

        /// <summary>Description text with fallback when absent.</summary>
        public string DisplayDescription => string.IsNullOrWhiteSpace(Product?.Description)
            ? "No description available."
            : Product.Description;

        partial void OnProductChanged(Product? value)
        {
            OnPropertyChanged(nameof(FormattedPrice));
            OnPropertyChanged(nameof(FormattedRating));
            OnPropertyChanged(nameof(DisplayDescription));
        }

        /// <summary>Navigates back to the catalog page.</summary>
        [RelayCommand]
        private static async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
