namespace SIGEBI.Business.DTOs
{
    public class ReporteDTO
    {
        public int TotalPrestamos { get; set; }
        public int TotalUsuarios { get; set; }
        public int TotalRecursos { get; set; }
        public int TotalPenalizaciones { get; set; }
        public int PrestamosAtrasados { get; set; }
        public DateTime FechaGeneracion { get; set; }
    }
}