using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using SIGEBI.Business.Interfaces.Services;

namespace SIGEBI.Infrastructure.Services
{
    public class EmailAdapter : IEmailAdapter
    {
        private readonly IConfiguration _configuration;

        public EmailAdapter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task EnviarAsync(string destinatario, string asunto, string cuerpo)
        {
            var host = _configuration["Smtp:Host"]!;
            var port = int.Parse(_configuration["Smtp:Port"]!);
            var usuario = _configuration["Smtp:Usuario"]!;
            var contrasena = _configuration["Smtp:Contrasena"]!;
            var nombreRemitente = _configuration["Smtp:NombreRemitente"]!;

            // Usamos SmtpClient aunque Microsoft dice que esta obsoleto (el profe dijo que para el TP safa)
            using var cliente = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(usuario, contrasena),
                EnableSsl = true
            };

            var mensaje = new MailMessage
            {
                From = new MailAddress(usuario, nombreRemitente),
                Subject = asunto,
                Body = cuerpo,
                IsBodyHtml = true
            };

            mensaje.To.Add(destinatario);

            await cliente.SendMailAsync(mensaje);
        }
    }
}