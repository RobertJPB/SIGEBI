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
        private readonly ApiService _apiService;

        [ObservableProperty]
        private ObservableCollection<PrestamoResponseDTO> _prestamos = new();

        [ObservableProperty]
        private string _mensajeError = string.Empty;

        [ObservableProperty]
        private bool _tieneError;

        [ObservableProperty]
        private string _contador = "0 préstamos";

        public GestionPrestamosViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Title = "Gestión de Préstamos";
        }

        [RelayCommand]
        public async Task CargarPrestamosAsync()
        {
            try
            {
                IsBusy = true;
                TieneError = false;
                var data = await _apiService.GetPrestamosAsync();
                Prestamos = new ObservableCollection<PrestamoResponseDTO>(data);
                Contador = $"{Prestamos.Count} préstamos";
            }
            catch (Exception ex)
            {
                MostrarError($"Error al cargar préstamos: {ex.Message}");
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
                await _apiService.DevolverPrestamoAsync(prestamoId);
                await CargarPrestamosAsync();
            }
            catch (Exception ex)
            {
                MostrarError($"Error al devolver préstamo: {ex.Message}");
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task EliminarAsync(Guid prestamoId)
        {
            try
            {
                IsBusy = true;
                await _apiService.EliminarPrestamoAsync(prestamoId);
                await CargarPrestamosAsync();
            }
            catch (Exception ex)
            {
                MostrarError($"Error al eliminar préstamo: {ex.Message}");
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
