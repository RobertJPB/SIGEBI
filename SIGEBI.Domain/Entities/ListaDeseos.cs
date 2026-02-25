using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGEBI.Domain.Entities
{
    public class ListaDeseos
    {
        public Guid Id { get; private set; }
        public Guid UsuarioId { get; private set; }
        public DateTime FechaCreacion { get; private set; }

        // Relación con los recursos
        private readonly List<RecursoBibliografico> _recursos = new();
        public IReadOnlyCollection<RecursoBibliografico> Recursos => _recursos.AsReadOnly();

        private ListaDeseos() { }

        public ListaDeseos(Guid usuarioId)
        {
            Id = Guid.NewGuid();
            UsuarioId = usuarioId;
            FechaCreacion = DateTime.UtcNow;
        }

        public void AgregarRecurso(RecursoBibliografico recurso)
        {
            if (!_recursos.Any(r => r.Id == recurso.Id))
                _recursos.Add(recurso);
        }
    }
}
