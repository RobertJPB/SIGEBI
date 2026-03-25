using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SIGEBI.Services;
using SIGEBI.Business.DTOs;

namespace SIGEBI.ViewModels
{
    public partial class AuditoriaViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private ObservableCollection<AuditoriaDTO> _auditorias = new();

        [ObservableProperty]
        private string _mensajeError = string.Empty;

        [ObservableProperty]
        private bool _tieneError;

        [ObservableProperty]
        private string _contador = "0 registros";

        public AuditoriaViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Title = "Auditoría del Sistema";
        }

        [RelayCommand]
        public async Task CargarAuditoriasAsync()
        {
            try
            {
                IsBusy = true;
                TieneError = false;
                var data = await _apiService.GetAuditoriasAsync();
                Auditorias = new ObservableCollection<AuditoriaDTO>(data);
                Contador = $"{Auditorias.Count} registros";
            }
            catch (Exception ex)
            {
                MostrarError($"Error al cargar auditorías: {ex.Message}");
            }
            finally
            {
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
