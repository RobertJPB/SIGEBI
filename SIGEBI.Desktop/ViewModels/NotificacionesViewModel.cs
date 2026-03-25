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
        private readonly ApiService _apiService;

        [ObservableProperty]
        private ObservableCollection<NotificacionDTO> _notificaciones = new();

        [ObservableProperty]
        private string _mensajeError = string.Empty;

        [ObservableProperty]
        private bool _tieneError;

        [ObservableProperty]
        private string _contador = "0 notificaciones";

        // ID del usuario autenticado para consultar sus notificaciones
        public Guid UsuarioId { get; set; }

        public NotificacionesViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Title = "Notificaciones";
        }

        [RelayCommand]
        public async Task CargarNotificacionesAsync()
        {
            try
            {
                IsBusy = true;
                TieneError = false;
                var data = await _apiService.GetNotificacionesAsync(UsuarioId);
                Notificaciones = new ObservableCollection<NotificacionDTO>(data);
                Contador = $"{Notificaciones.Count} notificaciones";
            }
            catch (Exception ex)
            {
                MostrarError($"Error al cargar notificaciones: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task EliminarAsync(Guid id)
        {
            try
            {
                IsBusy = true;
                await _apiService.EliminarNotificacionAsync(id);
                await CargarNotificacionesAsync();
            }
            catch (Exception ex)
            {
                MostrarError($"Error al eliminar notificación: {ex.Message}");
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
