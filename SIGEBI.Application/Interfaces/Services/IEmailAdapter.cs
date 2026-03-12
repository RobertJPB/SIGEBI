namespace SIGEBI.Business.Interfaces.Services
{
    public interface IEmailAdapter
    {
        Task EnviarAsync(string destinatario, string asunto, string cuerpo);
    }
}