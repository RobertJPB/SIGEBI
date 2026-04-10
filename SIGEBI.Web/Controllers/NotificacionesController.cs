using Microsoft.AspNetCore.Mvc;
using SIGEBI.Web.Services;
using SIGEBI.Web.Helpers;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SIGEBI.Web.Controllers
{
    public class NotificacionViewModel
    {
        public Guid Id { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public bool Leida => Estado == "Leida";
    }

    public class NotificacionesIndexViewModel
    {
        public List<NotificacionViewModel> Notificaciones { get; set; } = new();
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class NotificacionesController : Controller
    {
        private readonly INotificacionService _notificacionService;

        public NotificacionesController(INotificacionService notificacionService)
        {
            _notificacionService = notificacionService;
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
            var rolStr = HttpContext.Session.GetString("UsuarioRol");
            if (string.IsNullOrEmpty(usuarioIdStr) || !Guid.TryParse(usuarioIdStr, out var usuarioId) || string.IsNullOrEmpty(rolStr))
                return RedirectToAction("Login", "Auth");

            var model = new NotificacionesIndexViewModel();

            try
            {
                // Si es Admin o Bibliotecario, cargamos TODAS las notificaciones del sistema
                bool esAdmin = rolStr == "Administrador" || rolStr == "Bibliotecario";
                
                var dtos = esAdmin 
                    ? await _notificacionService.GetAllNotificacionesAsync(token)
                    : await _notificacionService.GetNotificacionesByUsuarioAsync(usuarioId, token);

                model.Notificaciones = dtos.Select(d => new NotificacionViewModel
                {
                    Id = d.Id,
                    Mensaje = d.Mensaje,
                    Estado = d.Estado,
                    Fecha = d.Fecha
                }).OrderByDescending(n => n.Fecha).ToList();
            }
            catch (Exception ex)
            {
                model.ErrorMessage = $"Error al obtener notificaciones: {ex.Message}";
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> MarcarLeida(Guid id)
        {
            try
            {
                var token = GetBearerToken();
                if (string.IsNullOrEmpty(token))
                    return Json(new { success = false, message = "Sesión expirada" });

                await _notificacionService.MarcarLeidaAsync(id, token);
                return Json(new { success = true });
            }
            catch (ApiException apiEx)
            {
                var error = await ApiErrorHelper.GetErrorMessageAsync(apiEx);
                return Json(new { success = false, message = "No se pudo actualizar: " + error });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> MarcarTodasLeidas()
        {
            try
            {
                var token = GetBearerToken();
                var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(usuarioIdStr) || !Guid.TryParse(usuarioIdStr, out var usuarioId))
                    return Json(new { success = false, message = "Sesión expirada" });

                await _notificacionService.MarcarTodasLeidasAsync(usuarioId, token);
                return Json(new { success = true });
            }
            catch (ApiException apiEx)
            {
                var error = await ApiErrorHelper.GetErrorMessageAsync(apiEx);
                return Json(new { success = false, message = "No se pudieron marcar todas: " + error });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            try
            {
                var token = GetBearerToken();
                var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(usuarioIdStr) || !Guid.TryParse(usuarioIdStr, out var usuarioId))
                    return Json(new { count = 0 });

                int count = await _notificacionService.GetUnreadCountAsync(usuarioId, token);
                return Json(new { count });
            }
            catch { }
            return Json(new { count = 0 });
        }
    }
}
