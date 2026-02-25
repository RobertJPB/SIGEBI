using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGEBI.Domain.Entities
{
    public class RecursoBibliografico
    {
        public Guid Id { get; private set; }
        public string Titulo { get; private set; } = null!;
        public string Autor { get; private set; } = null!;
        public int IdCategoria { get; private set; }
        public int Stock { get; private set; }
        public short Estado { get; private set; }

        private RecursoBibliografico() { }

        public RecursoBibliografico(string titulo, string autor, int idCategoria, int stockInicial)
        {
            if (stockInicial < 0) throw new ArgumentOutOfRangeException(nameof(stockInicial));

            Id = Guid.NewGuid();
            Titulo = titulo;
            Autor = autor;
            IdCategoria = idCategoria;
            Stock = stockInicial;
            Estado = 1;
        }

        public void DisminuirStock()
        {
            if (Stock <= 0) throw new InvalidOperationException("No hay stock disponible para este recurso.");
            Stock--;
        }

        public void AumentarStock() => Stock++;
    }
}
