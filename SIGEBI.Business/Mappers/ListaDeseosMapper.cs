using SIGEBI.Business.DTOs;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.Mappers
{
    public static class ListaDeseosMapper
    {
        public static ListaDeseosDTO ToDTO(ListaDeseos listaDeseos) => new()
        {
            Id = listaDeseos.Id,
            UsuarioId = listaDeseos.UsuarioId,
            UsuarioNombre = listaDeseos.Usuario?.Nombre ?? string.Empty,
            FechaCreacion = listaDeseos.FechaCreacion,
            Recursos = listaDeseos.Recursos.Select(RecursoMapper.ToDTO).ToList()
        };
    }
}