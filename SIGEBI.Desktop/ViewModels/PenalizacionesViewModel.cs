using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SIGEBI.Services;
using SIGEBI.Business.DTOs;

namespace SIGEBI.ViewModels
{
    public partial class PenalizacionesViewModel : BaseViewModel
    {
        private readonly IPenalizacionesApi _api;
        private readonly IUsuariosApi _usuariosApi;

        private IEnumerable<PenalizacionDTO> _todasLasPenalizaciones = Array.Empty<PenalizacionDTO>();

        [ObservableProperty] private ObservableCollection<PenalizacionDTO> _penalizaciones = new();
        [ObservableProperty] private ObservableCollection<UsuarioDTO> _usuarios = new();
        [ObservableProperty] private UsuarioDTO? _selectedUsuario;
        [ObservableProperty] private string _motivoManual = string.Empty;
        [ObservableProperty] private int _diasManual = 7;
        [ObservableProperty] private string _contador = "0 penalizaciones";
        [ObservableProperty] private string _searchQuery = string.Empty;

        partial void OnSearchQueryChanged(string value)
        {
            FiltrarResultados();
        }

        private void FiltrarResultados()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                Penalizaciones = new ObservableCollection<PenalizacionDTO>(_todasLasPenalizaciones);
            }
            else
            {
                var query = SearchQuery.ToLowerInvariant();
                var filtrados = _todasLasPenalizaciones.Where(p => 
                    (p.NombreUsuario?.ToLowerInvariant().Contains(query) == true) ||
                    (p.Motivo?.ToLowerInvariant().Contains(query) == true));
                
                Penalizaciones = new ObservableCollection<PenalizacionDTO>(filtrados);
            }
            Contador = $"{Penalizaciones.Count} penalizaciones visualizadas";
        }

        public PenalizacionesViewModel(IPenalizacionesApi api, IUsuariosApi usuariosApi)
        {
            _api = api;
            _usuariosApi = usuariosApi;
            Title = "Penalizaciones";
        }

        [RelayCommand]
        public async Task CargarPenalizacionesAsync()
        {
            try
            {
                IsBusy = true;
                LimpiarError();
                
                // Cargar penalizaciones
                var data = await _api.GetPenalizacionesAsync();
                _todasLasPenalizaciones = data;
                FiltrarResultados();
                
                // Cargar usuarios para el combo manual (solo una vez o si es necesario)
                if (Usuarios.Count == 0)
                {
                    var userList = await _usuariosApi.GetUsuariosAsync();
                    Usuarios = new ObservableCollection<UsuarioDTO>(userList.OrderBy(u => u.Nombre));
                }
            }
            catch (Exception ex)
            {
                await ManejarErrorAsync(ex, "cargar penalizaciones");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task FinalizarAsync(Guid id)
        {
            try
            {
                IsBusy = true;
                await _api.FinalizarPenalizacionAsync(id);
                await CargarPenalizacionesAsync();
            }
            catch (Exception ex)
            {
                await ManejarErrorAsync(ex, "finalizar penalización");
                IsBusy = false;
            }
        }


        [RelayCommand]
        public async Task EliminarAsync(Guid id)
        {
            var warning = System.Windows.MessageBox.Show("¿Estás seguro de eliminar esta penalización permanentemente?", "Confirmación", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning);
            if (warning != System.Windows.MessageBoxResult.Yes) return;

            try
            {
                IsBusy = true;
                await _api.EliminarPenalizacionAsync(id);
                await CargarPenalizacionesAsync();
            }
            catch (Exception ex)
            {
                await ManejarErrorAsync(ex, "eliminar penalización");
                IsBusy = false;
            }
        }


        [RelayCommand]
        public async Task EjecutarAplicarManualAsync()
        {
            if (SelectedUsuario == null)
            {
                System.Windows.MessageBox.Show("Por favor selecciona un usuario.", "Aviso", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(MotivoManual))
            {
                System.Windows.MessageBox.Show("Por favor ingresa un motivo.", "Aviso", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            try
            {
                IsBusy = true;
                var dto = new AplicarPenalizacionManualDTO
                {
                    UsuarioId = SelectedUsuario.Id,
                    Motivo = MotivoManual,
                    DiasPenalizacion = DiasManual
                };

                await _api.AplicarPenalizacionManualAsync(dto);
                
                // Limpiar form
                MotivoManual = string.Empty;
                SelectedUsuario = null;
                
                await CargarPenalizacionesAsync();
            }
            catch (Exception ex)
            {
                await ManejarErrorAsync(ex, "aplicar penalización manual");
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        public async Task AplicarManualAsync(AplicarPenalizacionManualDTO dto)
        {
            try
            {
                IsBusy = true;
                await _api.AplicarPenalizacionManualAsync(dto);
                await CargarPenalizacionesAsync();
            }
            catch (Exception ex)
            {
                await ManejarErrorAsync(ex, "aplicar penalización manual");
                IsBusy = false;
            }
        }
    }
}
