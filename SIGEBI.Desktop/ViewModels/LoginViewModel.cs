using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using SIGEBI.Services;

namespace SIGEBI.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly ISigebiApi _api;

        [ObservableProperty]
        private string _correo = string.Empty;

        // Callback para cerrar la ventana desde la vista
        public Action? OnLoginSuccess { get; set; }

        public LoginViewModel(ISigebiApi api)
        {
            _api = api;
            Title = "SIGEBI - Acceso Administrativo";
        }

        public async Task LoginAsync(string contrasena)
        {
            if (string.IsNullOrWhiteSpace(Correo) || string.IsNullOrWhiteSpace(contrasena))
            {
                MensajeError = "Correo y contraseña son obligatorios.";
                TieneError = true;
                return;
            }

            try
            {
                IsBusy = true;
                LimpiarError();

                var response = await _api.LoginAsync(new LoginRequestDTO(Correo, contrasena));

                // Persistir sesión
                SessionService.Token = response.Token;
                ExtraerDatosSesion(response.Token);

                OnLoginSuccess?.Invoke();
            }
            catch (Exception ex)
            {
                await ManejarErrorAsync(ex, "iniciar sesión");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Decodifica el JWT para extraer UsuarioId y Nombre del usuario
        /// sin requerir un endpoint adicional de perfil.
        /// </summary>
        private static void ExtraerDatosSesion(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);

                var sub = jwt.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == "nameid")?.Value;
                if (Guid.TryParse(sub, out var userId))
                    SessionService.UsuarioId = userId;

                var nombre = jwt.Claims.FirstOrDefault(c => c.Type == "name" || c.Type == "unique_name")?.Value;
                if (!string.IsNullOrWhiteSpace(nombre))
                    SessionService.NombreUsuario = nombre;
            }
            catch
            {
                // Si el JWT no tiene los claims esperados, la sesión continúa sin UsuarioId
            }
        }
    }
}

