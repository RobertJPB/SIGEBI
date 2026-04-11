using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using SIGEBI.Business.DTOs;

namespace SIGEBI.Services
{
    public interface IUsuariosApi
    {
        [Get("/api/Usuarios")]
        Task<List<UsuarioDTO>> GetUsuariosAsync();

        [Delete("/api/Usuarios/{id}")]
        Task EliminarUsuarioAsync(Guid id);

        [Put("/api/Usuarios/{id}/activar")]
        Task ActivarUsuarioAsync(Guid id);

        [Put("/api/Usuarios/{id}/desactivar")]
        Task DesactivarUsuarioAsync(Guid id, [Body] string motivo);

        [Put("/api/Usuarios/{id}/bloquear")]
        Task BloquearUsuarioAsync(Guid id, [Body] string motivo);

        [Put("/api/Usuarios/{id}/suspender")]
        Task SuspenderUsuarioAsync(Guid id);

        [Post("/api/Usuarios/{id}/rol")]
        Task CambiarRolUsuarioAsync(Guid id, [Body] int nuevoRol);
    }
}
