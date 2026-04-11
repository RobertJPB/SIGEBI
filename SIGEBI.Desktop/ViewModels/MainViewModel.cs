using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using SIGEBI.Services;

namespace SIGEBI.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        // Propiedad observable para el título cambiante de la sección central
        [ObservableProperty]
        private string _seccionActual = "Gestión Bibliográfica";

        [ObservableProperty]
        private int _notificacionesPendientes;

        public bool TienePendientes => NotificacionesPendientes > 0;

        private readonly INotificacionesApi _api;

        public MainViewModel(INotificacionesApi api)
        {
            _api = api;
            Title = "SIGEBI MVVM - Panel Administrativo";
            _ = ActualizarConteoNotificacionesAsync();
        }

        public async Task ActualizarConteoNotificacionesAsync()
        {
            if (SessionService.UsuarioId == Guid.Empty) return;
            try
            {
                NotificacionesPendientes = await _api.GetCantPendientesAsync(SessionService.UsuarioId);
                OnPropertyChanged(nameof(TienePendientes));
            }
            catch { /* Silencioso en el main */ }
        }
    }
}
