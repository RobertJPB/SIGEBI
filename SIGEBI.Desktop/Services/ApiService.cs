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

        // ── AGREGAR ──

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

        // ── EDITAR ──

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
            var response = await _httpClient.PutAsync($"api/Prestamos/{prestamoId}/devolver", null);
            response.EnsureSuccessStatusCode();
        }

        // ── HELPERS ──
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
            form.Add(new StringContent(request.FechaPublicacion?.ToString("yyyy-MM-dd") ?? ""), "FechaPublicacion");
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

    // ── DTOs ──

    public class CategoriaDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }

    public class RecursoDetalleDTO
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public string? CategoriaNombre { get; set; }
        public int Stock { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string TipoRecurso { get; set; } = string.Empty;
        public string? ISBN { get; set; }
        public string? Editorial { get; set; }
        public int? Anio { get; set; }
        public string? Genero { get; set; }
        public int? NumeroEdicion { get; set; }
        public string? ISSN { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        public string? Formato { get; set; }
        public string? Institucion { get; set; }
        public string? ImagenUrl { get; set; }
        public double? PromedioValoraciones { get; set; }
    }

    public class UsuarioDTO
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public int IdRol { get; set; }
        public short Estado { get; set; }
    }

    public class PrestamoRequestDTO
    {
        public Guid UsuarioId { get; set; }
        public Guid RecursoId { get; set; }
    }

    public class PrestamoResponseDTO
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty;
        public Guid RecursoId { get; set; }
        public string RecursoTitulo { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaLimite { get; set; }
        public string Estado { get; set; } = string.Empty;
    }

    public class AgregarLibroRequest
    {
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public int Stock { get; set; }
        public string? ISBN { get; set; }
        public string? Editorial { get; set; }
        public int? Anio { get; set; }
        public string? Genero { get; set; }
        public byte[]? ImagenBytes { get; set; }
        public string? ImagenNombre { get; set; }
    }

    public class AgregarRevistaRequest
    {
        public string Titulo { get; set; } = string.Empty;
        public string? Autor { get; set; }
        public int CategoriaId { get; set; }
        public int Stock { get; set; }
        public int NumeroEdicion { get; set; }
        public string? ISSN { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        public byte[]? ImagenBytes { get; set; }
        public string? ImagenNombre { get; set; }
    }

    public class AgregarDocumentoRequest
    {
        public string Titulo { get; set; } = string.Empty;
        public string? Autor { get; set; }
        public int CategoriaId { get; set; }
        public int Stock { get; set; }
        public string? Formato { get; set; }
        public string? Institucion { get; set; }
        public int? Anio { get; set; }
        public byte[]? ImagenBytes { get; set; }
        public string? ImagenNombre { get; set; }
    }
}
