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
        private readonly ISigebiApi _api;

        private IEnumerable<PenalizacionDTO> _todasLasPenalizaciones = Array.Empty<PenalizacionDTO>();

        [ObservableProperty] private ObservableCollection<PenalizacionDTO> _penalizaciones = new();
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

        public PenalizacionesViewModel(ISigebiApi api)
        {
            _api = api;
            Title = "Penalizaciones";
        }

        [RelayCommand]
        public async Task CargarPenalizacionesAsync()
        {
            try
            {
                IsBusy = true;
                LimpiarError();
                var data = await _api.GetPenalizacionesAsync();
                _todasLasPenalizaciones = data;
                FiltrarResultados();
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
        public async Task AplicarPenalizacionesAsync()
        {
            try
            {
                IsBusy = true;
                await _api.AplicarPenalizacionesAsync();
                await CargarPenalizacionesAsync();
            }
            catch (Exception ex)
            {
                await ManejarErrorAsync(ex, "aplicar penalizaciones");
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
        public void AbrirAplicarManual()
        {
            var modal = new SIGEBI.Views.Penalizaciones.AplicarPenalizacionWindow
            {
                DataContext = this,
                Owner = System.Windows.Application.Current.MainWindow
            };
            modal.ShowDialog();
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
