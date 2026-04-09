using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SIGEBI.Services;
using SIGEBI.Business.DTOs;

namespace SIGEBI.ViewModels
{
    public partial class GestionBibliograficaViewModel : BaseViewModel
    {
        private readonly ISigebiApi _api;
        private readonly ResourceUploadService _uploadService;

        [ObservableProperty]
        private ObservableCollection<RecursoDetalleDTO> _recursos = new();

        [ObservableProperty]
        private string _busqueda = string.Empty;

        [ObservableProperty]
        private string _contador = "0 recursos";

        public GestionBibliograficaViewModel(ISigebiApi api, ResourceUploadService uploadService)
        {
            _api = api;
            _uploadService = uploadService;
            Title = "Gestión Bibliográfica";
        }

        // ── Consultas via ISigebiApi (Refit) ──

        [RelayCommand]
        public async Task CargarRecursosAsync()
        {
            try
            {
                IsBusy = true;
                LimpiarError();
                var data = await _api.GetRecursosAsync();
                Recursos = new ObservableCollection<RecursoDetalleDTO>(data);
                Contador = $"{Recursos.Count} recursos";
            }
            catch (Exception ex)
            {
                await ManejarErrorAsync(ex, "cargar recursos");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task BuscarAsync()
        {
            var query = Busqueda?.Trim() ?? string.Empty;

            try
            {
                IsBusy = true;
                LimpiarError();
                var data = await _api.BuscarRecursosPorTituloAsync(query);
                Recursos = new ObservableCollection<RecursoDetalleDTO>(data);
                Contador = $"{Recursos.Count} resultados";
            }
            catch (Exception ex)
            {
                await ManejarErrorAsync(ex, "buscar recursos");
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
                await _api.EliminarRecursoAsync(id);
                await CargarRecursosAsync();
            }
            catch (Exception ex)
            {
                await ManejarErrorAsync(ex, "eliminar recurso");
                IsBusy = false;
            }
        }

        // ── Operaciones multipart (subida de archivos) ──
        // Delegadas a ResourceUploadService que gestiona el MultipartFormDataContent

        public async Task<RecursoDetalleDTO?> AgregarLibroAsync(AgregarLibroRequest request)
        {
            try
            {
                IsBusy = true;
                LimpiarError();
                var resultado = await _uploadService.AgregarLibroAsync(request);
                await CargarRecursosAsync();
                return resultado;
            }
            catch (Exception ex)
            {
                await ManejarErrorAsync(ex, "agregar libro");
                return null;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task<RecursoDetalleDTO?> AgregarRevistaAsync(AgregarRevistaRequest request)
        {
            try
            {
                IsBusy = true;
                LimpiarError();
                var resultado = await _uploadService.AgregarRevistaAsync(request);
                await CargarRecursosAsync();
                return resultado;
            }
            catch (Exception ex)
            {
                await ManejarErrorAsync(ex, "agregar revista");
                return null;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task<RecursoDetalleDTO?> AgregarDocumentoAsync(AgregarDocumentoRequest request)
        {
            try
            {
                IsBusy = true;
                LimpiarError();
                var resultado = await _uploadService.AgregarDocumentoAsync(request);
                await CargarRecursosAsync();
                return resultado;
            }
            catch (Exception ex)
            {
                await ManejarErrorAsync(ex, "agregar documento");
                return null;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task<RecursoDetalleDTO?> EditarLibroAsync(Guid id, AgregarLibroRequest request)
        {
            try
            {
                IsBusy = true;
                LimpiarError();
                var resultado = await _uploadService.EditarLibroAsync(id, request);
                await CargarRecursosAsync();
                return resultado;
            }
            catch (Exception ex)
            {
                await ManejarErrorAsync(ex, "editar libro");
                return null;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task<RecursoDetalleDTO?> EditarRevistaAsync(Guid id, AgregarRevistaRequest request)
        {
            try
            {
                IsBusy = true;
                LimpiarError();
                var resultado = await _uploadService.EditarRevistaAsync(id, request);
                await CargarRecursosAsync();
                return resultado;
            }
            catch (Exception ex)
            {
                await ManejarErrorAsync(ex, "editar revista");
                return null;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task<RecursoDetalleDTO?> EditarDocumentoAsync(Guid id, AgregarDocumentoRequest request)
        {
            try
            {
                IsBusy = true;
                LimpiarError();
                var resultado = await _uploadService.EditarDocumentoAsync(id, request);
                await CargarRecursosAsync();
                return resultado;
            }
            catch (Exception ex)
            {
                await ManejarErrorAsync(ex, "editar documento");
                return null;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
