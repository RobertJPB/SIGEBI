namespace SIGEBI.Business.DTOs
{
    /// <summary>
    /// Estructura simple para recibir un motivo o texto descriptivo en peticiones PUT/POST.
    /// Resuelve problemas de deserialización al enviar strings simples desde Refit.
    /// </summary>
    public class MotivoRequest
    {
        public string Motivo { get; set; } = string.Empty;
    }
}
