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
        private readonly ISigebiApi _api;

        [ObservableProperty]
        private ObservableCollection<PrestamoResponseDTO> _prestamos = new();

        [ObservableProperty]
        private string _contador = "0 préstamos";

        public GestionPrestamosViewModel(ISigebiApi api)
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
                Prestamos = new ObservableCollection<PrestamoResponseDTO>(data);
                Contador = $"{Prestamos.Count} préstamos";
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

