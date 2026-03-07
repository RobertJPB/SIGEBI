using SIGEBI.Business.Interfaces.Services;

namespace SIGEBI.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        public async Task EnviarAsync(string destinatario, string asunto, string cuerpo)
        {
            // Implementación SMTP ira aqui
            await Task.CompletedTask;
        }
    }
}