using System.IO;

namespace SIGEBI.Business.Interfaces.Services
{
    public interface IImagenService
    {
        Task<string?> GuardarImagenAsync(Stream? stream, string nombreArchivoOriginal, string subCarpeta);
        void EliminarImagen(string? urlRelativa);
        bool EsExtensionValida(string fileName);
    }
}


