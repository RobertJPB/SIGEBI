using System;
namespace SIGEBI.Business.DTOs
{
    public class PrestamoResponseDTO
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public Guid RecursoId { get; set; }
        public string TituloRecurso { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaDevolucionEstimada { get; set; }
        public DateTime? FechaDevolucionReal { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}