using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SIGEBI.Services;
using SIGEBI.Business.DTOs;

namespace SIGEBI.ViewModels
{
    public partial class GestionPrestamosViewModel : BaseViewModel
    {
        private readonly IPrestamosApi _api;

        private IEnumerable<PrestamoResponseDTO> _todosLosPrestamos = Array.Empty<PrestamoResponseDTO>();

        [ObservableProperty]
        private ObservableCollection<PrestamoResponseDTO> _prestamos = new();

        [ObservableProperty]
        private string _contador = "0 préstamos";

        [ObservableProperty]
        private string _searchQuery = string.Empty;

        partial void OnSearchQueryChanged(string value)
        {
            FiltrarResultados();
        }

        private void FiltrarResultados()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                Prestamos = new ObservableCollection<PrestamoResponseDTO>(_todosLosPrestamos);
            }
            else
            {
                var query = SearchQuery.ToLowerInvariant();
                var filtrados = _todosLosPrestamos.Where(p => 
                    (p.NombreUsuario?.ToLowerInvariant().Contains(query) == true) ||
                    (p.TituloRecurso?.ToLowerInvariant().Contains(query) == true) ||
                    (p.Estado?.ToLowerInvariant().Contains(query) == true));
                
                Prestamos = new ObservableCollection<PrestamoResponseDTO>(filtrados);
            }
            Contador = $"{Prestamos.Count} préstamos visualizados";
        }

        public GestionPrestamosViewModel(IPrestamosApi api)
        {
            _api = api;
            Title = "Gestión de Préstamos";
        }

        [RelayCommand]
        public async Task CargarPrestamosAsync()
        {
            try
            {
                IsBusy = true;
                LimpiarError();
                var data = await _api.GetPrestamosAsync();
                _todosLosPrestamos = data;
                FiltrarResultados();
            }
            catch (Exception ex)
            {
                await ManejarErrorAsync(ex, "cargar préstamos");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task DevolverAsync(Guid prestamoId)
        {
            try
            {
                IsBusy = true;
                await _api.DevolverPrestamoAsync(prestamoId);
                await CargarPrestamosAsync();
            }
            catch (Exception ex)
            {
                await ManejarErrorAsync(ex, "devolver préstamo");
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task EliminarAsync(Guid prestamoId)
        {
            try
            {
                IsBusy = true;
                await _api.EliminarPrestamoAsync(prestamoId);
                await CargarPrestamosAsync();
            }
            catch (Exception ex)
            {
                await ManejarErrorAsync(ex, "eliminar préstamo");
                IsBusy = false;
            }
        }
    }
}

