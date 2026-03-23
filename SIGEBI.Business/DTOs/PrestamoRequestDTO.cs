namespace SIGEBI.Business.DTOs
{
    public class PrestamoRequestDTO
    {
        public Guid UsuarioId { get; set; }
        public Guid RecursoId { get; set; }
        public DateTime? FechaDevolucionEstimada { get; set; }
    }
}
