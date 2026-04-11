using System.IO;
using Microsoft.AspNetCore.Hosting;
using SIGEBI.Business.Interfaces.Services;

namespace SIGEBI.Infrastructure.Services
{

    public class ImagenService : IImagenService
    {
        private readonly IWebHostEnvironment _env;
        private readonly string _imagesSubPath;

        public ImagenService(IWebHostEnvironment env, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _env = env;
            _imagesSubPath = configuration["Storage:ImagesPath"] ?? "imagenes";
        }

        public async Task<string?> GuardarImagenAsync(Stream? stream, string nombreArchivoOriginal, string subCarpeta)
        {
            if (stream == null || stream.Length == 0) return null;

            // Validaciones básicas antes de procesar
            if (!EsExtensionValida(nombreArchivoOriginal)) return null;

            // Definición de la ruta física
            var carpetaBase = Path.Combine(_env.WebRootPath ?? _env.ContentRootPath, _imagesSubPath, subCarpeta);
            Directory.CreateDirectory(carpetaBase);

            // Generación de nombre único
            var extension = Path.GetExtension(nombreArchivoOriginal).ToLowerInvariant();
            var nombreArchivo = $"{Guid.NewGuid()}{extension}";
            var rutaCompleta = Path.Combine(carpetaBase, nombreArchivo);

            using var fileStream = new FileStream(rutaCompleta, FileMode.Create);
            await stream.CopyToAsync(fileStream);

            // Retorno de la URL relativa para el cliente
            return $"/{_imagesSubPath}/{subCarpeta}/{nombreArchivo}";
        }

        public void EliminarImagen(string? urlRelativa)
        {
            if (string.IsNullOrWhiteSpace(urlRelativa)) return;

            try
            {
                // Convertimos URL relativa a ruta física
                var rutaLimpia = urlRelativa.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                var rutaCompleta = Path.Combine(_env.WebRootPath ?? _env.ContentRootPath, rutaLimpia);

                if (File.Exists(rutaCompleta))
                {
                    File.Delete(rutaCompleta);
                }
            }
            catch
            {
                // Fallback silencioso
            }
        }

        public bool EsExtensionValida(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return false;

            var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            return extensionesPermitidas.Contains(extension);
        }
    }
}
