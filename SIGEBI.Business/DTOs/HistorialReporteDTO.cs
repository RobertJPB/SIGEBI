namespace SIGEBI.Business.DTOs
{
    public class HistorialReporteDTO
    {
        public Guid Id { get; set; }
        public string Tipo { get; set; } = null!;
        public DateTime FechaGeneracion { get; set; }
        public string Parametros { get; set; } = null!;
        public string Resultado { get; set; } = null!;
    }
}
