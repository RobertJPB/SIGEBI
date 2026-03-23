using SIGEBI.Business.DTOs;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.Mappers
{
    public static class AuditoriaMapper
    {
        public static AuditoriaDTO ToDTO(Auditoria auditoria) => new()
        {
            Id = auditoria.Id,
            UsuarioId = auditoria.UsuarioId,
            NombreUsuario = auditoria.Usuario?.Nombre ?? string.Empty,
            Accion = auditoria.Accion,
            TablaAfectada = auditoria.TablaAfectada,
            Detalle = auditoria.Detalle,
            IpAddress = auditoria.IpAddress,
            FechaRegistro = auditoria.FechaRegistro
        };
    }
}