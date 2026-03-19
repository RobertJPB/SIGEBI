using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.DTOs;
using SIGEBI.Business.UseCases.Catalogo;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Domain.Enums.Seguridad;

// Permite a los usuarios calificar y dejar comentarios sobre los recursos de la biblioteca.
// Ayuda a otros usuarios a conocer la calidad y utilidad de la bibliografía disponible.
namespace SIGEBI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ValoracionesController : ControllerBase
    {
        private readonly ValoracionesUseCase _valoracionesUseCase;

        public ValoracionesController(ValoracionesUseCase valoracionesUseCase)
        {
            _valoracionesUseCase = valoracionesUseCase;
        }

        // Recupera el rol para aplicar las políticas de acceso al catálogo y sus interacciones.
        private RolUsuario ObtenerRolActual()
        {
            var rolClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (Enum.TryParse<RolUsuario>(rolClaim, out var rol))
                return rol;
            throw new UnauthorizedAccessException("Rol no identificado en el token.");
        }

        // Recupera el ID del usuario del token JWT.
        private Guid ObtenerUsuarioIdActual()
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(idClaim, out var id))
                return id;
            throw new UnauthorizedAccessException("Usuario no identificado en el token.");
        }

        // Lista todas las reseñas y calificaciones recibidas por un recurso específico.
        [HttpGet("recurso/{recursoId}")]
        public async Task<IActionResult> ObtenerPorRecurso(Guid recursoId)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "ver valoraciones");

            var valoraciones = await _valoracionesUseCase.ObtenerValoracionesPorRecursoAsync(recursoId);
            return Ok(valoraciones);
        }

        // Calcula y devuelve la puntuación media (estrellas) que los usuarios han dado a un recurso.
        [HttpGet("recurso/{recursoId}/promedio")]
        public async Task<IActionResult> ObtenerPromedio(Guid recursoId)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "ver promedio de valoraciones");

            var promedio = await _valoracionesUseCase.ObtenerPromedioAsync(recursoId);
            return Ok(promedio);
        }

        [HttpPost]
        public async Task<IActionResult> Agregar([FromBody] ValoracionDTO dto)
        {
            var rol = ObtenerRolActual();
            AccesoPolicy.ValidarAcceso(rol, AccesoPolicy.PuedeVerCatalogo(rol), "agregar valoración");

            var valoracion = await _valoracionesUseCase.AgregarValoracionAsync(
                dto.UsuarioId, dto.RecursoId, dto.Calificacion, dto.Comentario);
            return Ok(valoracion);
        }

        // Elimina una valoración. Solo la puede borrar el autor o un administrador/bibliotecario.
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(Guid id)
        {
            var rol = ObtenerRolActual();
            var usuarioId = ObtenerUsuarioIdActual();
            
            // Primero obtenemos la valoración para verificar autoría
            var valoracion = await _valoracionesUseCase.ObtenerPorIdAsync(id);
            
            // Si no es admin/bibliote, solo puede borrar su propia valoracion
            if (!AccesoPolicy.PuedeGestionarRecursos(rol) && valoracion.UsuarioId != usuarioId)
            {
                return Forbid("No tienes permiso para eliminar esta valoración.");
            }
            
            await _valoracionesUseCase.EliminarValoracionAsync(id);
            return Ok("Valoración eliminada correctamente.");
        }
    }
}
