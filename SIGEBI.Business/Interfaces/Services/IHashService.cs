namespace SIGEBI.Business.Interfaces.Services
{
    public interface IHashService
    {
        string Hash(string texto);
        bool Verificar(string texto, string hash);
    }
}