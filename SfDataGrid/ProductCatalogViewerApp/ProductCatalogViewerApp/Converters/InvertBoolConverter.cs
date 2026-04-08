using System.Globalization;

namespace ProductCatalogViewerApp.Converters
{
    /// <summary>
    /// Inverts a boolean value for XAML bindings (e.g., hide grid while loading).
    /// </summary>
    public class InvertBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value is bool b && !b;

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value is bool b && !b;
    }
}
