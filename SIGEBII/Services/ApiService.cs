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

        private const string BaseUrl = "https://localhost:7047/";

        public ApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }

        // ── TOKEN ──
        public void SetToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        // ── AUTH ──
        public async Task<string?> LoginAsync(string correo, string contrasena)
        {
            var payload = new { correo, contrasena };
            var response = await PostAsync("api/Auth/login", payload);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(json, _jsonOptions);
            return result.TryGetProperty("token", out var token) ? token.GetString() : null;
        }

        // ── RECURSOS ──
        public async Task<List<RecursoDetalleDTO>> GetRecursosAsync()
        {
            var response = await _httpClient.GetAsync("api/Recursos");
            response.EnsureSuccessStatusCode();
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

        public async Task<RecursoDetalleDTO?> AgregarLibroAsync(AgregarLibroRequest request)
        {
            var response = await PostAsync("api/Recursos/libro", request);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<RecursoDetalleDTO>(json, _jsonOptions);
        }

        public async Task<RecursoDetalleDTO?> AgregarRevistaAsync(AgregarRevistaRequest request)
        {
            var response = await PostAsync("api/Recursos/revista", request);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<RecursoDetalleDTO>(json, _jsonOptions);
        }

        public async Task<RecursoDetalleDTO?> AgregarDocumentoAsync(AgregarDocumentoRequest request)
        {
            var response = await PostAsync("api/Recursos/documento", request);
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
            var response = await PostAsync("api/Auth/registrar", dto);
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
            var response = await PostAsync("api/Prestamos", request);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PrestamoResponseDTO>(json, _jsonOptions);
        }

        public async Task DevolverPrestamoAsync(Guid prestamoId)
        {
            var response = await _httpClient.PutAsync($"api/Prestamos/{prestamoId}/devolver", null);
            response.EnsureSuccessStatusCode();
        }

        // ── HELPER ──
        private async Task<HttpResponseMessage> PostAsync(string url, object payload)
        {
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(url, content);
        }
    }

    // ── DTOs locales para WPF ──
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
        public int? NumeroEdicion { get; set; }
        public string? ISSN { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        public string? Formato { get; set; }
        public string? Institucion { get; set; }
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
        public string ISBN { get; set; } = string.Empty;
        public string Editorial { get; set; } = string.Empty;
        public int Anio { get; set; }
    }

    public class AgregarRevistaRequest
    {
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public int Stock { get; set; }
        public int NumeroEdicion { get; set; }
        public string ISSN { get; set; } = string.Empty;
        public DateTime FechaPublicacion { get; set; }
    }

    public class AgregarDocumentoRequest
    {
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public int Stock { get; set; }
        public string Formato { get; set; } = string.Empty;
        public string Institucion { get; set; } = string.Empty;
        public int Anio { get; set; }
    }
}
