using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.UseCases.Prestamos;
using SIGEBI.Business.DTOs;

namespace SIGEBI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrestamosController : ControllerBase
    {
        private readonly SolicitarPrestamoUseCase _solicitarPrestamo;

        public PrestamosController(SolicitarPrestamoUseCase solicitarPrestamo)
        {
            _solicitarPrestamo = solicitarPrestamo;
        }

        [HttpPost]
        public async Task<IActionResult> SolicitarPrestamo(PrestamoRequestDTO dto)
        {
            var resultado = await _solicitarPrestamo.EjecutarAsync(dto.UsuarioId, dto.RecursoId);
            return Ok(resultado);
        }
    }
}