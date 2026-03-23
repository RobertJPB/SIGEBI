using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SIGEBI.Services;
using SIGEBII.ViewModels;

namespace SIGEBI.ViewModels
{
    public partial class GestionBibliograficaViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private ObservableCollection<RecursoDetalleDTO> _recursos = new();

        [ObservableProperty]
        private string _busqueda = string.Empty;

        [ObservableProperty]
        private string _contador = "0 recursos";

        [ObservableProperty]
        private string _mensajeError = string.Empty;

        [ObservableProperty]
        private bool _tieneError;

        public GestionBibliograficaViewModel(ApiService apiService)
        {
            _apiService = apiService;
            Title = "Gestión Bibliográfica";
        }

        [RelayCommand]
        public async Task CargarRecursosAsync()
        {
            try
            {
                IsBusy = true;
                TieneError = false;

                var data = await _apiService.GetRecursosAsync();
                Recursos = new ObservableCollection<RecursoDetalleDTO>(data);
                Contador = $"{Recursos.Count} recursos";
            }
            catch (Exception ex)
            {
                MostrarError($"Error al cargar recursos: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task BuscarAsync()
        {
            if (string.IsNullOrWhiteSpace(Busqueda)) return;

            try
            {
                IsBusy = true;
                TieneError = false;

                var data = await _apiService.BuscarRecursosPorTituloAsync(Busqueda);
                Recursos = new ObservableCollection<RecursoDetalleDTO>(data);
                Contador = $"{Recursos.Count} resultados";
            }
            catch (Exception ex)
            {
                MostrarError($"Error al buscar: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task VerTodosAsync()
        {
            Busqueda = string.Empty;
            await CargarRecursosAsync();
        }

        [RelayCommand]
        public async Task EliminarAsync(Guid id)
        {
            try
            {
                IsBusy = true;
                await _apiService.EliminarRecursoAsync(id);
                await CargarRecursosAsync();
            }
            catch (Exception ex)
            {
                MostrarError($"Error al eliminar: {ex.Message}");
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
