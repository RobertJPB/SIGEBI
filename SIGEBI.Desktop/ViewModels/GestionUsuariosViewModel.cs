using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SIGEBI.Services;
using SIGEBI.Business.DTOs;

namespace SIGEBI.ViewModels
{
    public partial class GestionUsuariosViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private ObservableCollection<UsuarioDTO> _usuarios = new();

        [ObservableProperty]
        private UsuarioDTO? _selectedUsuario;

        [ObservableProperty]
        private string _busqueda = string.Empty;

        [ObservableProperty]
        private string _mensajeError = string.Empty;

        [ObservableProperty]
        private bool _tieneError;

        [ObservableProperty]
        private string _contador = "0 usuarios";

        public GestionUsuariosViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Title = "Gestión de Usuarios";
        }

        [RelayCommand]
        public async Task CargarUsuariosAsync()
        {
            try
            {
                IsBusy = true;
                TieneError = false;
                var data = await _apiService.GetUsuariosAsync();
                Usuarios = new ObservableCollection<UsuarioDTO>(data);
                Contador = $"{Usuarios.Count} usuarios";
            }
            catch (Exception ex)
            {
                MostrarError($"Error al cargar usuarios: {ex.Message}");
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
                await _apiService.EliminarUsuarioAsync(id);
                await CargarUsuariosAsync();
            }
            catch (Exception ex)
            {
                MostrarError($"Error al eliminar usuario: {ex.Message}");
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
