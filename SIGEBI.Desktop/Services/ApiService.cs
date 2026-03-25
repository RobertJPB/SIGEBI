using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SIGEBI.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // ── AUTH ──
        public async Task<string?> LoginAsync(string correo, string contrasena)
        {
            var payload = new { correo, contrasena };
            var response = await PostJsonAsync("api/Auth/login", payload);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(json, _jsonOptions);
            return result.TryGetProperty("token", out var token) ? token.GetString() : null;
        }

        // ── CATEGORÍAS ──
        public async Task<List<CategoriaDTO>> GetCategoriasAsync()
        {
            var response = await _httpClient.GetAsync("api/Categorias");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CategoriaDTO>>(json, _jsonOptions) ?? new();
        }

        // ── RECURSOS ──
        public async Task<List<RecursoDetalleDTO>> GetRecursosAsync()
        {
            var response = await _httpClient.GetAsync("api/Recursos");
            if (!response.IsSuccessStatusCode)
            {
                var errorJson = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error {(int)response.StatusCode}: {errorJson}");
            }
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<RecursoDetalleDTO>>(json, _jsonOptions) ?? new();
        }

        public async Task<List<RecursoDetalleDTO>> BuscarRecursosPorTituloAsync(string titulo)
        {
            var response = await _httpClient.GetAsync($"api/Recursos/buscar?titulo={Uri.EscapeDataString(titulo)}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<RecursoDetalleDTO>>(json, _jsonOptions) ?? new();
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

        public async Task EliminarRecursoAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/Recursos/{id}");
            response.EnsureSuccessStatusCode();
        }

        // ── USUARIOS ──
        public async Task<List<UsuarioDTO>> GetUsuariosAsync()
        {
            var response = await _httpClient.GetAsync("api/Usuarios");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<UsuarioDTO>>(json, _jsonOptions) ?? new();
        }

        public async Task AgregarUsuarioAsync(UsuarioDTO dto)
        {
            var response = await PostJsonAsync("api/Auth/registrar", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task EliminarUsuarioAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/Usuarios/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task ActualizarFotoPerfilAsync(byte[] imageBytes, string fileName)
        {
            using var content = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(imageBytes);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg"); // Genérico
            content.Add(fileContent, "foto", fileName);

            var response = await _httpClient.PostAsync("api/Usuarios/perfil/foto", content);
            response.EnsureSuccessStatusCode();
        }

        // ── PRÉSTAMOS ──
        public async Task<List<PrestamoResponseDTO>> GetPrestamosAsync()
        {
            var response = await _httpClient.GetAsync("api/Prestamos");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<PrestamoResponseDTO>>(json, _jsonOptions) ?? new();
        }

        public async Task<PrestamoResponseDTO?> SolicitarPrestamoAsync(PrestamoRequestDTO request)
        {
            var response = await PostJsonAsync("api/Prestamos", request);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PrestamoResponseDTO>(json, _jsonOptions);
        }

        public async Task DevolverPrestamoAsync(Guid prestamoId)
        {
            var response = await _httpClient.PutAsync($"api/Prestamos/devolver/{prestamoId}", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task EliminarPrestamoAsync(Guid prestamoId)
        {
            var response = await _httpClient.DeleteAsync($"api/Prestamos/{prestamoId}");
            response.EnsureSuccessStatusCode();
        }

        // ── PENALIZACIONES ──
        public async Task<List<PenalizacionDTO>> GetPenalizacionesAsync()
        {
            var response = await _httpClient.GetAsync("api/Penalizaciones");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<PenalizacionDTO>>(json, _jsonOptions) ?? new();
        }

        public async Task FinalizarPenalizacionAsync(Guid id)
        {
            var response = await _httpClient.PatchAsync($"api/Penalizaciones/{id}/finalizar", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task AplicarPenalizacionesAsync()
        {
            var response = await _httpClient.PostAsync("api/Penalizaciones/aplicar", null);
            response.EnsureSuccessStatusCode();
        }

        // ── NOTIFICACIONES ──
        public async Task<List<NotificacionDTO>> GetNotificacionesAsync(Guid usuarioId)
        {
            var response = await _httpClient.GetAsync($"api/Notificaciones/usuario/{usuarioId}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<NotificacionDTO>>(json, _jsonOptions) ?? new();
        }

        public async Task EliminarNotificacionAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/Notificaciones/{id}");
            response.EnsureSuccessStatusCode();
        }

        // ── AUDITORÍA ──
        public async Task<List<AuditoriaDTO>> GetAuditoriasAsync()
        {
            var response = await _httpClient.GetAsync("api/Auditoria");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<AuditoriaDTO>>(json, _jsonOptions) ?? new();
        }

        // ── HELPERS PRIVADOS ──
        private MultipartFormDataContent BuildLibroForm(AgregarLibroRequest request)
        {
            var form = new MultipartFormDataContent();
            form.Add(new StringContent(request.Titulo), "Titulo");
            form.Add(new StringContent(request.Autor), "Autor");
            form.Add(new StringContent(request.CategoriaId.ToString()), "CategoriaId");
            form.Add(new StringContent(request.Stock.ToString()), "Stock");
            form.Add(new StringContent(request.ISBN ?? ""), "ISBN");
            form.Add(new StringContent(request.Editorial ?? ""), "Editorial");
            form.Add(new StringContent(request.Anio?.ToString() ?? "0"), "Anio");
            if (request.Descripcion != null)
                form.Add(new StringContent(request.Descripcion), "Descripcion");
            if (request.Genero != null)
                form.Add(new StringContent(request.Genero), "Genero");
            if (request.ImagenBytes != null && request.ImagenNombre != null)
                form.Add(new ByteArrayContent(request.ImagenBytes), "Imagen", request.ImagenNombre);
            return form;
        }

        private MultipartFormDataContent BuildRevistaForm(AgregarRevistaRequest request)
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
            if (request.Descripcion != null)
                form.Add(new StringContent(request.Descripcion), "Descripcion");
            if (request.ImagenBytes != null && request.ImagenNombre != null)
                form.Add(new ByteArrayContent(request.ImagenBytes), "Imagen", request.ImagenNombre);
            return form;
        }

        private MultipartFormDataContent BuildDocumentoForm(AgregarDocumentoRequest request)
        {
            var form = new MultipartFormDataContent();
            form.Add(new StringContent(request.Titulo), "Titulo");
            form.Add(new StringContent(request.Autor ?? ""), "Autor");
            form.Add(new StringContent(request.CategoriaId.ToString()), "CategoriaId");
            form.Add(new StringContent(request.Stock.ToString()), "Stock");
            form.Add(new StringContent(request.Formato ?? ""), "Formato");
            form.Add(new StringContent(request.Institucion ?? ""), "Institucion");
            form.Add(new StringContent(request.Anio?.ToString() ?? "0"), "Anio");
            if (request.Descripcion != null)
                form.Add(new StringContent(request.Descripcion), "Descripcion");
            if (request.ImagenBytes != null && request.ImagenNombre != null)
                form.Add(new ByteArrayContent(request.ImagenBytes), "Imagen", request.ImagenNombre);
            return form;
        }

        private async Task<HttpResponseMessage> PostJsonAsync(string url, object payload)
        {
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(url, content);
        }
    }
}
