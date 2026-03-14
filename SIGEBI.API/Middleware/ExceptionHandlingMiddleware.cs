using System.Text.Json;

namespace SIGEBI.API.Middleware
{
    // Principio SOLID (SRP - Responsabilidad Única):
    // Su única responsabilidad es atrapar excepciones (try/catch global) y formatearlas como JSON.
    // Le quita esta responsabilidad a los Controladores para que ellos solo manejen flujo HTTP.
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Si explota algo en cualquier lado, cae aca y le armamos un JSON bonito
                await ManejarExcepcionAsync(context, ex);
            }
        }

        private static Task ManejarExcepcionAsync(HttpContext context, Exception ex)
        {
            var statusCode = ex switch
            {
                KeyNotFoundException => StatusCodes.Status404NotFound,
                UnauthorizedAccessException => StatusCodes.Status403Forbidden,
                ArgumentException => StatusCodes.Status400BadRequest,
                InvalidOperationException => StatusCodes.Status422UnprocessableEntity,
                _ => StatusCodes.Status500InternalServerError
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var respuesta = new
            {
                status = statusCode,
                mensaje = ex.Message
            };

            return context.Response.WriteAsJsonAsync(respuesta);
        }
    }
}