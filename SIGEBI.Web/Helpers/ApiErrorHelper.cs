using System.Text.Json;
using Refit;

namespace SIGEBI.Web.Helpers
{
    public static class ApiErrorHelper
    {
        public static Task<string> GetErrorMessageAsync(ApiException apiEx)
        {
            if (string.IsNullOrWhiteSpace(apiEx.Content))
                return Task.FromResult(apiEx.ReasonPhrase ?? "Error de red");

            return Task.FromResult(ParseJsonError(apiEx.Content) ?? apiEx.ReasonPhrase ?? "Error en el servidor");
        }

        public static async Task<string> GetErrorMessageAsync(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content))
                return response.ReasonPhrase ?? "Error desconocido";

            return ParseJsonError(content) ?? response.ReasonPhrase ?? "Error desconocido";
        }

        private static string? ParseJsonError(string json)
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var element = JsonSerializer.Deserialize<JsonElement>(json, options);

                // Prioridad 1: RFC 7807 Detail (Lo que usa nuestro nuevo Middleware)
                if (element.TryGetProperty("detail", out var detail))
                    return detail.GetString();

                // Prioridad 2: Formato antiguo "message"
                if (element.TryGetProperty("message", out var message))
                    return message.GetString();

                // Prioridad 3: RFC 7807 Title
                if (element.TryGetProperty("title", out var title))
                    return title.GetString();

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
