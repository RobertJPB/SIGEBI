using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using SIGEBI.Business.DTOs;

namespace SIGEBI.Services
{
    // ── DTOs exclusivos del cliente Desktop ──

    /// <summary>Payload enviado al endpoint de autenticación.</summary>
    public record LoginRequestDTO(string Correo, string Contrasena);

    /// <summary>Respuesta del endpoint de autenticación (token JWT).</summary>
    public record LoginResponseDTO(string Token);

    public interface ISigebiApi
    {
        // ── AUTH ──
        [Post("/api/Auth/login")]
        Task<LoginResponseDTO> LoginAsync([Body] LoginRequestDTO request);

        // ── CATEGORÍAS ──
        [Get("/api/Categorias")]
        Task<List<CategoriaDTO>> GetCategoriasAsync();

        // ── RECURSOS ──
        [Get("/api/Recursos")]
        Task<List<RecursoDetalleDTO>> GetRecursosAsync();

        [Get("/api/Recursos/buscar")]
        Task<List<RecursoDetalleDTO>> BuscarRecursosPorTituloAsync([Query] string titulo);

        [Delete("/api/Recursos/{id}")]
        Task EliminarRecursoAsync(Guid id);

        // ── USUARIOS ──
        [Get("/api/Usuarios")]
        Task<List<UsuarioDTO>> GetUsuariosAsync();

        [Post("/api/Auth/registrar")]
        Task RegistrarUsuarioAsync([Body] UsuarioDTO dto);

        [Delete("/api/Usuarios/{id}")]
        Task EliminarUsuarioAsync(Guid id);

        [Put("/api/Usuarios/{id}/activar")]
        Task ActivarUsuarioAsync(Guid id);

        [Put("/api/Usuarios/{id}/desactivar")]
        Task DesactivarUsuarioAsync(Guid id);

        [Put("/api/Usuarios/{id}/bloquear")]
        Task BloquearUsuarioAsync(Guid id);

        [Post("/api/Usuarios/{id}/rol")]
        Task CambiarRolUsuarioAsync(Guid id, [Body] int nuevoRol);

        // ── PRÉSTAMOS ──
        [Get("/api/Prestamos")]
        Task<List<PrestamoResponseDTO>> GetPrestamosAsync();

        [Post("/api/Prestamos")]
        Task<PrestamoResponseDTO> SolicitarPrestamoAsync([Body] PrestamoRequestDTO request);

        [Put("/api/Prestamos/devolver/{prestamoId}")]
        Task DevolverPrestamoAsync(Guid prestamoId);

        [Delete("/api/Prestamos/{prestamoId}")]
        Task EliminarPrestamoAsync(Guid prestamoId);

        // ── PENALIZACIONES ──
        [Get("/api/Penalizaciones")]
        Task<List<PenalizacionDTO>> GetPenalizacionesAsync();

        [Patch("/api/Penalizaciones/{id}/finalizar")]
        Task FinalizarPenalizacionAsync(Guid id);

        [Delete("/api/Penalizaciones/{id}")]
        Task EliminarPenalizacionAsync(Guid id);

        [Post("/api/Penalizaciones")]
        Task AplicarPenalizacionManualAsync([Body] AplicarPenalizacionManualDTO dto);

        [Post("/api/Penalizaciones/aplicar")]
        Task AplicarPenalizacionesAsync();

        // ── NOTIFICACIONES ──
        [Get("/api/Notificaciones/usuario/{usuarioId}")]
        Task<List<NotificacionDTO>> GetNotificacionesAsync(Guid usuarioId);

        [Delete("/api/Notificaciones/{id}")]
        Task EliminarNotificacionAsync(Guid id);

        // ── AUDITORÍA ──
        [Get("/api/Auditoria")]
        Task<List<AuditoriaDTO>> GetAuditoriasAsync();
    }
}
