using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BoilerplateGenerator.Converters
{
    public class BooleanToInverseVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool objectValue && objectValue ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility objectVisibility && objectVisibility == Visibility.Collapsed;
        }
    }
}
