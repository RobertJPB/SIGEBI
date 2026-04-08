using System;
using System.Globalization;
using System.Windows.Data;

namespace SIGEBI.Converters
{
    public class ApiUrlConverter : IValueConverter
    {
        private const string BaseUrl = "https://localhost:7047/";

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string path && !string.IsNullOrWhiteSpace(path))
            {
                if (path.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    return path;
                }

                // Asegurar que no haya doble diagonal
                var cleanPath = path.TrimStart('/');
                return $"{BaseUrl}{cleanPath}";
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
