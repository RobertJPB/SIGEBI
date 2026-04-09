using System;
using System.Globalization;
using System.Windows.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SIGEBI.Desktop.Converters
{
    public class ApiUrlConverter : IValueConverter
    {
        private static string? _baseUrl;

        public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            if (string.IsNullOrEmpty(_baseUrl))
            {
                var config = App.Current.Services.GetService<IConfiguration>();
                _baseUrl = config?["ApiSettings:BaseUrl"] ?? "http://localhost:5031/";
            }

            string? relativePath = value.ToString();
            if (string.IsNullOrEmpty(relativePath)) return null;

            // Limpiamos barras duplicadas
            string baseUri = _baseUrl!.TrimEnd('/');
            string path = relativePath!.TrimStart('/');

            return $"{baseUri}/{path}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
