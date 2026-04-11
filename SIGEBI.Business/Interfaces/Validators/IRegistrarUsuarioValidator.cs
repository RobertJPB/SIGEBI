using SIGEBI.Business.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIGEBI.Business.Interfaces.Validators
{
    public interface IRegistrarUsuarioValidator
    {
        Task<List<string>> ValidarAsync(UsuarioDTO dto);
        Task<bool> EsValidoAsync(UsuarioDTO dto);
    }
}
