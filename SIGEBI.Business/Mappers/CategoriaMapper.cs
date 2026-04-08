using SIGEBI.Business.DTOs;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.Mappers
{
    public static class CategoriaMapper
    {
        public static CategoriaDTO ToDTO(Categoria categoria)
        {
            return new CategoriaDTO
            {
                Id = categoria.Id,
                Nombre = categoria.Nombre,
                Estado = categoria.Estado.ToString()
            };
        }
    }
}
