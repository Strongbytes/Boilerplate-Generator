using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BoilerplateGenerator.Converters
{
    public class InverseVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility objectVisibility && objectVisibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility objectVisibility && objectVisibility == Visibility.Visible;
        }
    }
}
