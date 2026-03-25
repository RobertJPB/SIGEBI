using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using SIGEBI.Services;
using SIGEBI.Business.DTOs;

namespace SIGEBI.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private string _correo = string.Empty;

        [ObservableProperty]
        private string _mensajeError = string.Empty;

        [ObservableProperty]
        private bool _tieneError;

        // Callback para cerrar la ventana desde la vista
        public Action? OnLoginSuccess { get; set; }

        public LoginViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Title = "SIGEBI - Acceso Administrativo";
        }

        public async Task LoginAsync(string contrasena)
        {
            if (string.IsNullOrWhiteSpace(Correo) || string.IsNullOrWhiteSpace(contrasena))
            {
                MostrarError("Correo y contraseña son obligatorios.");
                return;
            }

            try
            {
                IsBusy = true;
                TieneError = false;

                var token = await _apiService.LoginAsync(Correo, contrasena);

                if (string.IsNullOrWhiteSpace(token))
                {
                    MostrarError("Correo o contraseña incorrectos.");
                    return;
                }

                SessionService.Token = token;

                OnLoginSuccess?.Invoke();
            }
            catch (Exception ex)
            {
                MostrarError($"No se pudo conectar con el servidor. {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void MostrarError(string mensaje)
        {
            MensajeError = mensaje;
            TieneError = true;
        }
    }
}
