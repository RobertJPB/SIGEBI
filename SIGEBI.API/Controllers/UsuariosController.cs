using Microsoft.AspNetCore.Mvc;

namespace SIGEBI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        [HttpGet]
        public IActionResult ObtenerUsuarios()
        {
            var usuarios = new string[]
            {
                "Usuario 1",
                "Usuario 2",
                "Usuario 3"
            };

            return Ok(usuarios);
        }
    }
}
