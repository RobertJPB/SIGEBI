using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Services;
using SIGEBI.Web.Helpers;
using Refit;

namespace SIGEBI.Web.Controllers
{
    public class ListaDeseosItemViewModel
    {
        public Guid Id { get; set; }
        public string RecursoTitulo { get; set; } = string.Empty;
        public string RecursoAutor { get; set; } = string.Empty;
        public string CategoriaNombre { get; set; } = string.Empty;
        public string? ImagenUrl { get; set; }
    }

    public class ListaDeseosViewModel
    {
        public List<ListaDeseosItemViewModel> Items { get; set; } = new();
        public string Token { get; set; } = string.Empty;
        public string ApiBaseUrl { get; set; } = string.Empty;
    }

    public class ListaDeseosController : Controller
    {
        private readonly IListaDeseosService _wishlistService;
        private readonly IConfiguration _configuration;

        public ListaDeseosController(IListaDeseosService wishlistService, IConfiguration configuration)
        {
            _wishlistService = wishlistService;
            _configuration = configuration;
        }

        private string GetBearerToken()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            return string.IsNullOrWhiteSpace(token) ? string.Empty : $"Bearer {token}";
        }

        public async Task<IActionResult> Index()
        {
            var token = GetBearerToken();
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr) || !Guid.TryParse(usuarioIdStr, out var usuarioId))
                return RedirectToAction("Login", "Auth");

            var model = new ListaDeseosViewModel 
            { 
                Token = token,
                ApiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7047/"
            };

            try
            {
                var apiData = await _wishlistService.GetListaDeseosAsync(usuarioId, token);

                if (apiData != null && apiData.Recursos != null)
                {
                    model.Items = apiData.Recursos.Select(r => new ListaDeseosItemViewModel
                    {
                        Id = r.Id,
                        RecursoTitulo = r.Titulo,
                        RecursoAutor = r.Autor,
                        CategoriaNombre = r.CategoriaNombre,
                        ImagenUrl = r.ImagenUrl
                    }).ToList();
                }
            }
            catch { /* Items queda vacío si hay error */ }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Quitar(Guid id)
        {
            try
            {
                var token = GetBearerToken();
                var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");

                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(usuarioIdStr))
                    return Json(new { success = false, message = "Sesiones expirada" });

                await _wishlistService.QuitarDeListaDeseosAsync(Guid.Parse(usuarioIdStr), id, token);
                return Json(new { success = true });
            }
            catch (ApiException apiEx)
            {
                var error = await ApiErrorHelper.GetErrorMessageAsync(apiEx);
                return Json(new { success = false, message = "No se pudo quitar: " + error });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
