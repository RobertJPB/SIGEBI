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
            FechaCreacion = listaDeseos.FechaCreacion,
            Recursos = listaDeseos.Recursos.Select(RecursoMapper.ToDTO).ToList()
        };
    }
}