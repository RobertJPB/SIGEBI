using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SIGEBI.Services;

namespace SIGEBI.Views.GestionBibliografica
{
    public partial class GestionBibliograficaPage : Page
    {
        private readonly ApiService _api;

        public GestionBibliograficaPage()
        {
            InitializeComponent();
            _api = SessionService.ApiService!;
            Loaded += async (s, e) => await CargarRecursos();
        }

        private async Task CargarRecursos()
        {
            try
            {
                MensajePanel.Visibility = Visibility.Collapsed;
                var recursos = await _api.GetRecursosAsync();
                GridRecursos.ItemsSource = recursos;
                TxtContador.Text = $"{recursos.Count} recursos";
            }
            catch (Exception ex)
            {
                MostrarError($"Error al cargar recursos: {ex.Message}");
            }
        }

        private async void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            var busqueda = TxtBusqueda.Text.Trim();
            if (string.IsNullOrWhiteSpace(busqueda)) return;
            try
            {
                var recursos = await _api.BuscarRecursosPorTituloAsync(busqueda);
                GridRecursos.ItemsSource = recursos;
                TxtContador.Text = $"{recursos.Count} resultados";
            }
            catch (Exception ex)
            {
                MostrarError($"Error al buscar: {ex.Message}");
            }
        }

        private async void BtnVerTodos_Click(object sender, RoutedEventArgs e)
        {
            TxtBusqueda.Text = string.Empty;
            await CargarRecursos();
        }

        private void TxtBusqueda_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                BtnBuscar_Click(sender, e);
        }

        private void BtnAgregarLibro_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AgregarLibroDialog(_api) { Owner = Window.GetWindow(this) };
            if (dialog.ShowDialog() == true) _ = CargarRecursos();
        }

        private void BtnAgregarRevista_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AgregarRevistaDialog(_api) { Owner = Window.GetWindow(this) };
            if (dialog.ShowDialog() == true) _ = CargarRecursos();
        }

        private void BtnAgregarDocumento_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AgregarDocumentoDialog(_api) { Owner = Window.GetWindow(this) };
            if (dialog.ShowDialog() == true) _ = CargarRecursos();
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is RecursoDetalleDTO recurso)
            {
                bool result = false;
                var tipo = recurso.TipoRecurso?.ToLower() ?? "";

                if (tipo.Contains("libro"))
                {
                    var dialog = new EditarLibroDialog(_api, recurso) { Owner = Window.GetWindow(this) };
                    result = dialog.ShowDialog() == true;
                }
                else if (tipo.Contains("revista"))
                {
                    var dialog = new EditarRevistaDialog(_api, recurso) { Owner = Window.GetWindow(this) };
                    result = dialog.ShowDialog() == true;
                }
                else if (tipo.Contains("documento"))
                {
                    var dialog = new EditarDocumentoDialog(_api, recurso) { Owner = Window.GetWindow(this) };
                    result = dialog.ShowDialog() == true;
                }

                if (result) _ = CargarRecursos();
            }
        }

        private async void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Guid id)
            {
                var confirm = MessageBox.Show(
                    "¿Estás seguro de que deseas eliminar este recurso?",
                    "Confirmar eliminación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (confirm == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _api.EliminarRecursoAsync(id);
                        await CargarRecursos();
                    }
                    catch (Exception ex)
                    {
                        MostrarError($"Error al eliminar: {ex.Message}");
                    }
                }
            }
        }

        private void MostrarError(string mensaje)
        {
            MensajeText.Text = mensaje;
            MensajePanel.Visibility = Visibility.Visible;
        }
    }
}
