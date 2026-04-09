using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Refit;
using System.Net;

namespace SIGEBI.Web.Filters
{
    /// <summary>
    /// Filtro global que captura errores de la API (específicamente 403 Forbidden).
    /// Si la API rechaza una petición porque el usuario fue suspendido, este filtro
    /// redirige automáticamente a la página de Logout para cerrar la sesión web.
    /// </summary>
    public class UserStatusActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Ejecutar la acción del controlador
            var resultContext = await next();

            // Verificar si ocurrió una excepción de Refit (API)
            if (resultContext.Exception is ApiException apiEx)
            {
                if ((int)apiEx.StatusCode == 403)
                {
                    resultContext.ExceptionHandled = true;
                    
                    var message = "Su acceso ha sido restringido por motivos administrativos.";
                    
                    try 
                    {
                        // Intentar parsear el JSON de la API
                        var response = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(apiEx.Content ?? "{}");
                        if (response.TryGetProperty("message", out var msgElement))
                        {
                            message = msgElement.GetString() ?? message;
                        }
                    }
                    catch { /* Fallback al mensaje por defecto */ }
                    
                    resultContext.Result = new RedirectToActionResult("Logout", "Auth", new { message });
                }
            }
        }
    }
}
