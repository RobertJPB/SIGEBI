using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SIGEBI.Services;
using SIGEBI.Business.DTOs;

namespace SIGEBI.ViewModels
{
    public partial class NotificacionesViewModel : BaseViewModel
    {
        private readonly INotificacionesApi _api;

        [ObservableProperty]
        private ObservableCollection<NotificacionDTO> _notificaciones = new();

        [ObservableProperty]
        private string _contador = "0 notificaciones";

        public NotificacionesViewModel(INotificacionesApi api)
        {
            _api = api;
            Title = "Notificaciones";
        }

        [RelayCommand]
        public async Task CargarNotificacionesAsync()
        {
            try
            {
                IsBusy = true;
                LimpiarError();

                // El UsuarioId se obtiene de la sesión iniciada — no se necesita parámetro externo
                var data = await _api.GetNotificacionesAsync(SessionService.UsuarioId);
                Notificaciones = new ObservableCollection<NotificacionDTO>(data);
                Contador = $"{Notificaciones.Count} notificaciones";
            }
            catch (Exception ex)
            {
                await ManejarErrorAsync(ex, "cargar notificaciones");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task MarcarLeidaAsync(Guid id)
        {
            try
            {
                IsBusy = true;
                await _api.MarcarNotificacionLeidaAsync(id);
                await CargarNotificacionesAsync();
                
                // Actualizar contador global
                ActualizarNotificacionesGlobal();
            }
            catch (Exception ex)
            {
                await ManejarErrorAsync(ex, "marcar notificación como leída");
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task MarcarTodasLeidasAsync()
        {
            try
            {
                IsBusy = true;
                await _api.MarcarTodasComoLeidasAsync(SessionService.UsuarioId);
                await CargarNotificacionesAsync();
                
                // Actualizar contador global
                ActualizarNotificacionesGlobal();
            }
            catch (Exception ex)
            {
                await ManejarErrorAsync(ex, "marcar todas como leídas");
                IsBusy = false;
            }
        }

        private void ActualizarNotificacionesGlobal()
        {
            if (App.Current.Services.GetService(typeof(MainViewModel)) is MainViewModel mainVm)
            {
                _ = mainVm.ActualizarConteoNotificacionesAsync();
            }
        }

        [RelayCommand]
        public async Task EliminarAsync(Guid id)
        {
            try
            {
                IsBusy = true;
                await _api.EliminarNotificacionAsync(id);
                await CargarNotificacionesAsync();
            }
            catch (Exception ex)
            {
                await ManejarErrorAsync(ex, "eliminar notificación");
                IsBusy = false;
            }
        }
    }
}

