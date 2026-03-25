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
        private readonly ApiService _apiService;

        [ObservableProperty]
        private ObservableCollection<PenalizacionDTO> _penalizaciones = new();

        [ObservableProperty]
        private string _mensajeError = string.Empty;

        [ObservableProperty]
        private bool _tieneError;

        [ObservableProperty]
        private string _contador = "0 penalizaciones";

        public PenalizacionesViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Title = "Penalizaciones";
        }

        [RelayCommand]
        public async Task CargarPenalizacionesAsync()
        {
            try
            {
                IsBusy = true;
                TieneError = false;
                var data = await _apiService.GetPenalizacionesAsync();
                Penalizaciones = new ObservableCollection<PenalizacionDTO>(data);
                Contador = $"{Penalizaciones.Count} penalizaciones";
            }
            catch (Exception ex)
            {
                MostrarError($"Error al cargar penalizaciones: {ex.Message}");
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
                await _apiService.FinalizarPenalizacionAsync(id);
                await CargarPenalizacionesAsync();
            }
            catch (Exception ex)
            {
                MostrarError($"Error al finalizar penalización: {ex.Message}");
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task AplicarPenalizacionesAsync()
        {
            try
            {
                IsBusy = true;
                await _apiService.AplicarPenalizacionesAsync();
                await CargarPenalizacionesAsync();
            }
            catch (Exception ex)
            {
                MostrarError($"Error al aplicar penalizaciones: {ex.Message}");
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
