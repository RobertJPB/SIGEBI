using System.Text.Json;

namespace SIGEBI.API.Middleware
{
    /// Middleware global para la captura de excepciones.
    /// Centraliza el manejo de errores para devolver respuestas JSON consistentes al cliente.
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
                // Continúa la ejecución de la petición
                await _next(context);
            }
            catch (Exception ex)
            {
                // Si ocurre un error no controlado en cualquier parte de la ejecución (UseCase, Repository, etc.),
                // el flujo cae aquí para ser procesado de forma unificada.
                await ManejarExcepcionAsync(context, ex);
            }
        }

        /// Determina el código de estado HTTP adecuado basándose en el tipo de excepción capturada.
        private static Task ManejarExcepcionAsync(HttpContext context, Exception ex)
        {
            var statusCode = ex switch
            {
                KeyNotFoundException => StatusCodes.Status404NotFound, // Recurso no encontrado
                UnauthorizedAccessException => StatusCodes.Status403Forbidden, // Sin permisos (RBAC)
                ArgumentException => StatusCodes.Status400BadRequest, // Datos inválidos
                InvalidOperationException => StatusCodes.Status422UnprocessableEntity, // Regla de negocio rota
                _ => StatusCodes.Status500InternalServerError // Error inesperado del servidor
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var respuesta = new
            {
                status = statusCode,
                mensaje = ex.Message,
                detalle = ex.InnerException?.Message // Agregamos el detalle del error interno para debug
            };

            return context.Response.WriteAsJsonAsync(respuesta);
        }
    }
}