using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Refit;

namespace SIGEBI.ViewModels
{
    /// <summary>
    /// Clase base para todos los ViewModels del sistema.
    /// Centraliza: estado de carga (IsBusy), manejo de errores y título de pantalla.
    /// ObservableObject ya implementa INotifyPropertyChanged.
    /// </summary>
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _title = string.Empty;

        /// <summary>Mensaje de error amigable para mostrar en la UI.</summary>
        [ObservableProperty]
        private string _mensajeError = string.Empty;

        /// <summary>Controla la visibilidad del panel de error en la UI.</summary>
        [ObservableProperty]
        private bool _tieneError;

        /// <summary>
        /// Maneja errores de red y de negocio de forma centralizada.
        /// Extrae el mensaje del cuerpo de respuesta si el error proviene de la API (Refit.ApiException).
        /// </summary>
        /// <param name="ex">Excepción capturada.</param>
        /// <param name="accion">Descripción de la acción fallida (ej: "cargar usuarios").</param>
        protected Task ManejarErrorAsync(Exception ex, string accion)
        {
            string mensaje;

            if (ex is ApiException apiEx)
            {
                // Extrae el error de negocio enviado por la API (400, 404, 500, etc.)
                string detalle = apiEx.ReasonPhrase;
                
                if (!string.IsNullOrWhiteSpace(apiEx.Content))
                {
                    try 
                    {
                        using var doc = System.Text.Json.JsonDocument.Parse(apiEx.Content);
                        var root = doc.RootElement;
                        
                        if (root.TryGetProperty("detail", out var detailProp))
                            detalle = detailProp.GetString() ?? detalle;
                        else if (root.TryGetProperty("message", out var msgProp))
                            detalle = msgProp.GetString() ?? detalle;
                        else if (root.TryGetProperty("title", out var titleProp))
                            detalle = titleProp.GetString() ?? detalle;
                    }
                    catch { /* Fallback al reason phrase */ }
                }

                mensaje = $"No se pudo {accion}: {detalle}";
            }
            else
            {
                // Error de red, timeout o servidor caído
                mensaje = $"Error de conexión al {accion}. Verifique que el servidor esté disponible.";
            }

            MensajeError = mensaje;
            TieneError = true;

            return Task.CompletedTask;
        }

        /// <summary>Limpia el estado de error.</summary>
        protected void LimpiarError()
        {
            MensajeError = string.Empty;
            TieneError = false;
        }
    }
}

