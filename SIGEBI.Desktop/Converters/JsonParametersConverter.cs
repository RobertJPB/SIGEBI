using System;
using System.Globalization;
using System.Text.Json;
using System.Windows.Data;

namespace SIGEBI.Desktop.Converters
{
    public class JsonParametersConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string jsonStr || string.IsNullOrWhiteSpace(jsonStr) || jsonStr == "0")
                return "Ninguno";

            if (jsonStr == "{}")
                return "Todos";

            try
            {
                using var doc = JsonDocument.Parse(jsonStr);
                var root = doc.RootElement;
                
                var items = new System.Collections.Generic.List<string>();

                foreach (var prop in root.EnumerateObject())
                {
                    string propValue = prop.Value.ToString();
                    
                    if (string.IsNullOrEmpty(propValue))
                        continue;

                    // Formatear fechas si detectamos el patron
                    if (propValue.Contains("T") && DateTime.TryParse(propValue, out DateTime dt))
                    {
                        propValue = dt.ToString("dd/MM/yyyy");
                    }
                    else if (prop.Name.Equals("Top", StringComparison.OrdinalIgnoreCase))
                    {
                        propValue = $"Top {propValue}";
                    }

                    items.Add($"{prop.Name}: {propValue}");
                }

                if (items.Count == 0) return "Todos";
                
                return string.Join(" | ", items);
            }
            catch
            {
                return jsonStr;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
