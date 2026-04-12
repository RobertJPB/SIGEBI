using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SIGEBI.Desktop.Converters
{
    /// <summary>
    /// Convierte un booleano a Visibilidad de forma invertida:
    /// True -> Collapsed / Hidden
    /// False -> Visible
    /// </summary>
    public class InvertedBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = (bool)value;
            return boolValue ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility = (Visibility)value;
            return visibility != Visibility.Visible;
        }
    }
}
