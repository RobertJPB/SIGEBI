using System;
using System.Collections.Generic;
using System.Linq;

namespace SIGEBI.Domain.Entities
{
    public class ListaDeseos
    {
        public Guid Id { get; private set; }
        public Guid UsuarioId { get; private set; }
        public DateTime FechaCreacion { get; private set; }

        private readonly List<RecursoBibliografico> _recursos = new();
        public IReadOnlyCollection<RecursoBibliografico> Recursos => _recursos.AsReadOnly();

        private ListaDeseos() { }

        public ListaDeseos(Guid usuarioId, DateTime fechaCreacionUtc)
        {
            if (usuarioId == Guid.Empty)
                throw new ArgumentException("UsuarioId inválido.", nameof(usuarioId));

            Id = Guid.NewGuid();
            UsuarioId = usuarioId;
            FechaCreacion = fechaCreacionUtc;
        }

        public void AgregarRecurso(RecursoBibliografico recurso)
        {
            if (recurso is null)
                throw new ArgumentNullException(nameof(recurso));

            if (_recursos.Any(r => r.Id == recurso.Id))
                return;

            _recursos.Add(recurso);
        }

        public void RemoverRecurso(Guid recursoId)
        {
            if (recursoId == Guid.Empty)
                throw new ArgumentException("RecursoId inválido.", nameof(recursoId));

            var existente = _recursos.FirstOrDefault(r => r.Id == recursoId);
            if (existente is null) return;

            _recursos.Remove(existente);
        }
    }
}