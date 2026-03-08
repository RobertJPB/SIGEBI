using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.DTOs;
using SIGEBI.Business.UseCases.Catalogo;

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

        // Obtener valoraciones de un recurso
        [HttpGet("recurso/{recursoId}")]
        public async Task<IActionResult> ObtenerPorRecurso(Guid recursoId)
        {
            var valoraciones = await _valoracionesUseCase.ObtenerValoracionesPorRecursoAsync(recursoId);
            return Ok(valoraciones);
        }

        // Obtener promedio de calificaciones de un recurso
        [HttpGet("recurso/{recursoId}/promedio")]
        public async Task<IActionResult> ObtenerPromedio(Guid recursoId)
        {
            var promedio = await _valoracionesUseCase.ObtenerPromedioAsync(recursoId);
            return Ok(promedio);
        }

        // Agregar valoracion
        [HttpPost]
        [Authorize(Roles = "Estudiante")]
        public async Task<IActionResult> Agregar([FromBody] ValoracionDTO dto)
        {
            var valoracion = await _valoracionesUseCase.AgregarValoracionAsync(
                dto.UsuarioId, dto.RecursoId, dto.Calificacion, dto.Comentario);
            return Ok(valoracion);
        }
    }
}