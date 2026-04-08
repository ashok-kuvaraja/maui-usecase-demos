using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace StudentGradesDashboard.Converters
{
    public class GradeToColorConverter : IValueConverter
    {
        // Passing threshold can be adjusted from XAML if needed
        public double PassingThreshold { get; set; } = 50.0;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
                return Colors.Red;

            if (double.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double grade))
            {
                return grade >= PassingThreshold ? Colors.Green : Colors.Red;
            }

            return Colors.Red;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
