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

        [ObservableProperty] private ObservableCollection<PenalizacionDTO> _penalizaciones = new();
        [ObservableProperty] private string _contador = "0 penalizaciones";

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
                Penalizaciones = new ObservableCollection<PenalizacionDTO>(data);
                Contador = $"{Penalizaciones.Count} penalizaciones";
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

    }
}
