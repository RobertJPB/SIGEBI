using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace SIGEBI.Services
{
    public class ResourceUploadService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ResourceUploadService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // ── AGREGAR RECURSOS ──

        public async Task<RecursoDetalleDTO?> AgregarLibroAsync(AgregarLibroRequest request)
        {
            var form = BuildLibroForm(request);
            var response = await _httpClient.PostAsync("api/Recursos/libro", form);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<RecursoDetalleDTO>(json, _jsonOptions);
        }

        public async Task<RecursoDetalleDTO?> AgregarRevistaAsync(AgregarRevistaRequest request)
        {
            var form = BuildRevistaForm(request);
            var response = await _httpClient.PostAsync("api/Recursos/revista", form);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<RecursoDetalleDTO>(json, _jsonOptions);
        }

        public async Task<RecursoDetalleDTO?> AgregarDocumentoAsync(AgregarDocumentoRequest request)
        {
            var form = BuildDocumentoForm(request);
            var response = await _httpClient.PostAsync("api/Recursos/documento", form);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<RecursoDetalleDTO>(json, _jsonOptions);
        }

        // ── EDITAR RECURSOS ──

        public async Task<RecursoDetalleDTO?> EditarLibroAsync(Guid id, AgregarLibroRequest request)
        {
            var form = BuildLibroForm(request);
            var response = await _httpClient.PutAsync($"api/Recursos/libro/{id}", form);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<RecursoDetalleDTO>(json, _jsonOptions);
        }

        public async Task<RecursoDetalleDTO?> EditarRevistaAsync(Guid id, AgregarRevistaRequest request)
        {
            var form = BuildRevistaForm(request);
            var response = await _httpClient.PutAsync($"api/Recursos/revista/{id}", form);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<RecursoDetalleDTO>(json, _jsonOptions);
        }

        public async Task<RecursoDetalleDTO?> EditarDocumentoAsync(Guid id, AgregarDocumentoRequest request)
        {
            var form = BuildDocumentoForm(request);
            var response = await _httpClient.PutAsync($"api/Recursos/documento/{id}", form);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<RecursoDetalleDTO>(json, _jsonOptions);
        }

        // ── BUILDERS PRIVADOS ──

        private static MultipartFormDataContent BuildLibroForm(AgregarLibroRequest request)
        {
            var form = new MultipartFormDataContent();
            form.Add(new StringContent(request.Titulo), "Titulo");
            form.Add(new StringContent(request.Autor), "Autor");
            form.Add(new StringContent(request.CategoriaId.ToString()), "CategoriaId");
            form.Add(new StringContent(request.Stock.ToString()), "Stock");
            form.Add(new StringContent(request.ISBN ?? ""), "ISBN");
            form.Add(new StringContent(request.Editorial ?? ""), "Editorial");
            form.Add(new StringContent(request.Anio?.ToString() ?? "0"), "Anio");
            form.Add(new StringContent(request.NumeroPaginas?.ToString() ?? "0"), "NumeroPaginas");
            if (request.Descripcion != null)
                form.Add(new StringContent(request.Descripcion), "Descripcion");
            if (request.Genero != null)
                form.Add(new StringContent(request.Genero), "Genero");
            if (request.ImagenBytes != null && request.ImagenNombre != null)
                form.Add(new ByteArrayContent(request.ImagenBytes), "Imagen", request.ImagenNombre);
            return form;
        }

        private static MultipartFormDataContent BuildRevistaForm(AgregarRevistaRequest request)
        {
            var form = new MultipartFormDataContent();
            form.Add(new StringContent(request.Titulo), "Titulo");
            form.Add(new StringContent(request.Autor ?? ""), "Autor");
            form.Add(new StringContent(request.CategoriaId.ToString()), "CategoriaId");
            form.Add(new StringContent(request.Stock.ToString()), "Stock");
            form.Add(new StringContent(request.ISSN ?? ""), "ISSN");
            form.Add(new StringContent(request.NumeroEdicion.ToString()), "NumeroEdicion");
            form.Add(new StringContent(request.Anio.ToString()), "Anio");
            form.Add(new StringContent(request.Editorial ?? ""), "Editorial");
            form.Add(new StringContent(request.NumeroPaginas?.ToString() ?? "0"), "NumeroPaginas");
            if (request.Descripcion != null)
                form.Add(new StringContent(request.Descripcion), "Descripcion");
            if (request.ImagenBytes != null && request.ImagenNombre != null)
                form.Add(new ByteArrayContent(request.ImagenBytes), "Imagen", request.ImagenNombre);
            return form;
        }

        private static MultipartFormDataContent BuildDocumentoForm(AgregarDocumentoRequest request)
        {
            var form = new MultipartFormDataContent();
            form.Add(new StringContent(request.Titulo), "Titulo");
            form.Add(new StringContent(request.Autor ?? ""), "Autor");
            form.Add(new StringContent(request.CategoriaId.ToString()), "CategoriaId");
            form.Add(new StringContent(request.Stock.ToString()), "Stock");
            form.Add(new StringContent(request.Formato ?? ""), "Formato");
            form.Add(new StringContent(request.Institucion ?? ""), "Institucion");
            form.Add(new StringContent(request.Anio?.ToString() ?? "0"), "Anio");
            form.Add(new StringContent(request.NumeroPaginas?.ToString() ?? "0"), "NumeroPaginas");
            if (request.Descripcion != null)
                form.Add(new StringContent(request.Descripcion), "Descripcion");
            if (request.ImagenBytes != null && request.ImagenNombre != null)
                form.Add(new ByteArrayContent(request.ImagenBytes), "Imagen", request.ImagenNombre);
            return form;
        }
    }
}

