using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SIGEBI.Services;
using SIGEBI.Business.DTOs;

namespace SIGEBI.Views.GestionBibliografica
{
    public partial class GestionBibliograficaPage : Page
    {
        private readonly ResourceUploadService _uploadService;
        private readonly ISigebiApi _api;
        private readonly SIGEBI.ViewModels.GestionBibliograficaViewModel _viewModel;

        public GestionBibliograficaPage()
        {
            InitializeComponent();
            _uploadService = (ResourceUploadService)SIGEBI.App.Current.Services.GetService(typeof(ResourceUploadService))!;
            _api = (ISigebiApi)SIGEBI.App.Current.Services.GetService(typeof(ISigebiApi))!;

            _viewModel = (SIGEBI.ViewModels.GestionBibliograficaViewModel)SIGEBI.App.Current.Services.GetService(typeof(SIGEBI.ViewModels.GestionBibliograficaViewModel))!;
            DataContext = _viewModel;

            Loaded += (s, e) => _ = _viewModel.CargarRecursosAsync();
        }

        private void TxtBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                _viewModel.BuscarCommand.Execute(null);
        }

        private void BtnAgregarLibro_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AgregarLibroDialog(_uploadService, _api) { Owner = Window.GetWindow(this) };
            if (dialog.ShowDialog() == true) _ = _viewModel.CargarRecursosAsync();
        }

        private void BtnAgregarRevista_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AgregarRevistaDialog(_uploadService, _api) { Owner = Window.GetWindow(this) };
            if (dialog.ShowDialog() == true) _ = _viewModel.CargarRecursosAsync();
        }

        private void BtnAgregarDocumento_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AgregarDocumentoDialog(_uploadService, _api) { Owner = Window.GetWindow(this) };
            if (dialog.ShowDialog() == true) _ = _viewModel.CargarRecursosAsync();
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is RecursoDetalleDTO recurso)
            {
                bool result = false;
                var tipo = recurso.TipoRecurso?.ToLower() ?? "";

                if (tipo.Contains("libro"))
                {
                    var dialog = new EditarLibroDialog(_uploadService, _api, recurso) { Owner = Window.GetWindow(this) };
                    result = dialog.ShowDialog() == true;
                }
                else if (tipo.Contains("revista"))
                {
                    var dialog = new EditarRevistaDialog(_uploadService, _api, recurso) { Owner = Window.GetWindow(this) };
                    result = dialog.ShowDialog() == true;
                }
                else if (tipo.Contains("documento"))
                {
                    var dialog = new EditarDocumentoDialog(_uploadService, _api, recurso) { Owner = Window.GetWindow(this) };
                    result = dialog.ShowDialog() == true;
                }

                if (result) _ = _viewModel.CargarRecursosAsync();
            }
        }
    }
}
