using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SIGEBI.Services;
using SIGEBI.Business.DTOs;

namespace SIGEBI.ViewModels
{
    public partial class GestionUsuariosViewModel : BaseViewModel
    {
        private readonly ISigebiApi _api;
        private List<UsuarioDTO> _allUsuarios = new();

        [ObservableProperty] private ObservableCollection<UsuarioDTO> _usuarios = new();
        [ObservableProperty] private UsuarioDTO? _selectedUsuario;
        [ObservableProperty] private string _busqueda = string.Empty;
        [ObservableProperty] private string _contador = "0 usuarios";

        public GestionUsuariosViewModel(ISigebiApi api)
        {
            _api = api;
            Title = "Gestión de Usuarios";
        }

        partial void OnBusquedaChanged(string value) => FiltrarUsuarios();

        private void FiltrarUsuarios()
        {
            if (string.IsNullOrWhiteSpace(Busqueda))
            {
                Usuarios = new ObservableCollection<UsuarioDTO>(_allUsuarios);
            }
            else
            {
                var filtered = _allUsuarios.Where(u => 
                    u.Nombre.Contains(Busqueda, StringComparison.OrdinalIgnoreCase) || 
                    u.Correo.Contains(Busqueda, StringComparison.OrdinalIgnoreCase)).ToList();
                Usuarios = new ObservableCollection<UsuarioDTO>(filtered);
            }
            Contador = $"{Usuarios.Count} usuarios";
        }

        [RelayCommand]
        public async Task CargarUsuariosAsync()
        {
            try
            {
                IsBusy = true;
                LimpiarError();
                _allUsuarios = await _api.GetUsuariosAsync();
                FiltrarUsuarios();
            }
            catch (Exception ex)
            {
                await ManejarErrorAsync(ex, "cargar usuarios");
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
                await _api.EliminarUsuarioAsync(id);
                await CargarUsuariosAsync();
            }
            catch (Exception ex)
            {
                await ManejarErrorAsync(ex, "eliminar usuario");
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task ActivarAsync(Guid id)
        {
            try
            {
                IsBusy = true;
                await _api.ActivarUsuarioAsync(id);
                await CargarUsuariosAsync();
            }
            catch (Exception ex)
            {
                await ManejarErrorAsync(ex, "activar usuario");
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task DesactivarAsync(Guid id)
        {
            try
            {
                IsBusy = true;
                await _api.DesactivarUsuarioAsync(id);
                await CargarUsuariosAsync();
            }
            catch (Exception ex)
            {
                await ManejarErrorAsync(ex, "desactivar usuario");
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task BloquearAsync(Guid id)
        {
            try
            {
                IsBusy = true;
                await _api.BloquearUsuarioAsync(id);
                await CargarUsuariosAsync();
            }
            catch (Exception ex)
            {
                await ManejarErrorAsync(ex, "bloquear usuario");
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task CambiarRolAsync(object parameter)
        {
            if (parameter is object[] values && values.Length == 2 && values[0] is Guid id && values[1] is int roleId)
            {
                try
                {
                    IsBusy = true;
                    await _api.CambiarRolUsuarioAsync(id, roleId);
                    await CargarUsuariosAsync();
                }
                catch (Exception ex)
                {
                    await ManejarErrorAsync(ex, "cambiar rol");
                    IsBusy = false;
                }
            }
        }

        [RelayCommand]
        public async Task RegistrarUsuarioAsync(UsuarioDTO nuevo)
        {
            try
            {
                IsBusy = true;
                await _api.RegistrarUsuarioAsync(nuevo);
                await CargarUsuariosAsync();
            }
            catch (Exception ex)
            {
                await ManejarErrorAsync(ex, "registrar usuario");
                IsBusy = false;
            }
        }

    }
}
