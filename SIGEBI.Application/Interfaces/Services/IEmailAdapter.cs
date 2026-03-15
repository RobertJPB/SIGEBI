namespace SIGEBI.Business.Interfaces.Services
{
    // Principio SOLID (ISP - Segregación de Interfaces):
    // Este contrato es super conciso. Solo define como enviar un email.
    // Si queremos enviar SMS manejariamos otra interfaz separada.
    public interface IEmailAdapter
    {
        Task EnviarAsync(string destinatario, string asunto, string cuerpo);
    }
}