using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGEBI.Business.UseCases.Catalogo;

namespace SIGEBI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriasController : ControllerBase
    {
        private readonly CategoriasUseCase _categoriasUseCase;

        public CategoriasController(CategoriasUseCase categoriasUseCase)
        {
            _categoriasUseCase = categoriasUseCase;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodas()
        {
            var categorias = await _categoriasUseCase.ObtenerTodasAsync();
            return Ok(categorias);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var categoria = await _categoriasUseCase.ObtenerPorIdAsync(id);
            return Ok(categoria);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Crear([FromBody] string nombre)
        {
            var categoria = await _categoriasUseCase.CrearAsync(nombre);
            return Ok(categoria);
        }
    }
}