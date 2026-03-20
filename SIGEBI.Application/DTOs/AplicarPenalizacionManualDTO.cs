namespace SIGEBI.Business.DTOs
{
    public class AplicarPenalizacionManualDTO
    {
        public Guid UsuarioId { get; set; }
        public Guid? PrestamoId { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public int DiasPenalizacion { get; set; }
    }
}
