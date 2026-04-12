using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Refit;
using System.Net;

namespace SIGEBI.Web.Filters
{
    public class UserStatusActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Ejecutar la acción del controlador
            var resultContext = await next();

            // Verificar si ocurrió una excepción
            if (resultContext.Exception != null)
            {
                // CASO 1: Error de conexión / API Caída (Mantenimiento)
                if (SIGEBI.Web.Helpers.ApiErrorHelper.EsErrorDeConexion(resultContext.Exception))
                {
                    resultContext.ExceptionHandled = true;
                    resultContext.Result = new ViewResult
                    {
                        ViewName = "Mantenimiento"
                    };
                    return;
                }

                // CASO 2: Error de Prohibición (Usuario suspendido/bloqueado)
                if (resultContext.Exception is ApiException apiEx && (int)apiEx.StatusCode == 403)
                {
                    resultContext.ExceptionHandled = true;
                    
                    var message = "Su acceso ha sido restringido por motivos administrativos.";
                    
                    try 
                    {
                        var response = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(apiEx.Content ?? "{}");
                        if (response.TryGetProperty("message", out var msgElement))
                        {
                            message = msgElement.GetString() ?? message;
                        }
                    }
                    catch { }
                    
                    resultContext.Result = new RedirectToActionResult("Logout", "Auth", new { message });
                }
            }
        }
    }
}

