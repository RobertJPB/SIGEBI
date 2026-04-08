namespace SIGEBI.Business.DTOs
{
    public class ListaDeseosDTO
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public List<RecursoDetalleDTO> Recursos { get; set; } = new();
    }
}
