using System.Linq;
using System.Threading.Tasks;
using SIGEBI.Business.Interfaces.Persistance;

namespace SIGEBI.Business.UseCases.Usuarios
{
    public class PenalizacionesUseCase
    {
        private readonly IPrestamoRepository _prestamoRepository;

        public PenalizacionesUseCase(IPrestamoRepository prestamoRepository)
        {
            _prestamoRepository = prestamoRepository;
        }

        public async Task AplicarPenalizacion(Guid usuarioId)
        {
            var prestamos = await _prestamoRepository.GetAtrasadosAsync();

            var prestamosUsuario = prestamos.Where(p => p.UsuarioId == usuarioId);

            if (prestamosUsuario.Any())
            {
                // lógica de penalización
            }
        }
    }
}