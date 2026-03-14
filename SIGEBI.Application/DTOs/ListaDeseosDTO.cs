namespace SIGEBI.Business.DTOs
{
    public class ListaDeseosDTO
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public List<RecursoDetalleDTO> Recursos { get; set; } = new();
    }
}