using System.IO;

namespace SIGEBI.Business.Interfaces.Services
{
    /// <summary>
    /// Servicio para la gestión de archivos multimedia (portadas, fotos de perfil, etc.).
    /// </summary>
    public interface IImagenService
    {
        /// <summary>
        /// Guarda una imagen desde un flujo de datos.
        /// </summary>
        /// <param name="stream">Flujo del archivo.</param>
        /// <param name="nombreArchivoOriginal">Nombre con extensión del archivo original.</param>
        /// <param name="subCarpeta">Subcarpeta destino.</param>
        /// <returns>URL relativa del recurso guardado.</returns>
        Task<string?> GuardarImagenAsync(Stream? stream, string nombreArchivoOriginal, string subCarpeta);

        /// <summary>
        /// Elimina una imagen físicamente del almacenamiento.
        /// </summary>
        void EliminarImagen(string? urlRelativa);

        /// <summary>
        /// Valida si la extensión y el tamaño son correctos.
        /// </summary>
        bool EsExtensionValida(string fileName);
    }
}
