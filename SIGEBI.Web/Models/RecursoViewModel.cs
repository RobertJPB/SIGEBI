namespace SIGEBI.Web.Models
{
    public class RecursoViewModel
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public string CategoriaNombre { get; set; } = string.Empty;
        public int Stock { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string TipoRecurso { get; set; } = string.Empty;
    }
}