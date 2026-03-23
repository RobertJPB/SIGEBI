using System;
using System.IO;
using System.Windows;
using SIGEBI.Services;

namespace SIGEBI.Views.GestionBibliografica
{
    public partial class EditarRevistaDialog : Window
    {
        private readonly ApiService _api;
        private readonly RecursoDetalleDTO _recurso;
        private byte[]? _imagenBytes;
        private string? _imagenNombre;

        public EditarRevistaDialog() : this((ApiService)SIGEBII.App.Current.Services.GetService(typeof(ApiService))!, new RecursoDetalleDTO()) { }

        public EditarRevistaDialog(ApiService api, RecursoDetalleDTO recurso)
        {
            InitializeComponent();
            _api = api;
            _recurso = recurso;
            Loaded += async (s, e) =>
            {
                try
                {
                    var categorias = await _api.GetCategoriasAsync();
                    CmbCategoria.ItemsSource = categorias;
                    CmbCategoria.SelectedValue = recurso.CategoriaId;
                }
                catch
                {
                    ErrorText.Text = "No se pudieron cargar las categorias.";
                    ErrorPanel.Visibility = Visibility.Visible;
                }

                TxtTitulo.Text = recurso.Titulo;
                TxtAutor.Text = recurso.Autor;
                TxtISSN.Text = recurso.ISSN ?? "";
                TxtNumeroEdicion.Text = recurso.NumeroEdicion?.ToString() ?? "";
                TxtFechaPublicacion.Text = recurso.FechaPublicacion?.ToString("yyyy-MM-dd") ?? "";
                TxtStock.Text = recurso.Stock.ToString();

                if (!string.IsNullOrEmpty(recurso.ImagenUrl))
                {
                    TxtRutaImagen.Text = recurso.ImagenUrl;
                    try
                    {
                        ImgPreview.Source = new System.Windows.Media.Imaging.BitmapImage(
                            new Uri($"https://localhost:7047{recurso.ImagenUrl}"));
                    }
                    catch { }
                }
            };
        }

        private void BtnSeleccionarImagen_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Imagenes|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                Title = "Seleccionar imagen de portada"
            };
            if (dialog.ShowDialog() == true)
            {
                _imagenBytes = File.ReadAllBytes(dialog.FileName);
                _imagenNombre = Path.GetFileName(dialog.FileName);
                TxtRutaImagen.Text = _imagenNombre;
                ImgPreview.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(dialog.FileName));
            }
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            ErrorPanel.Visibility = Visibility.Collapsed;

            if (string.IsNullOrWhiteSpace(TxtTitulo.Text) ||
                string.IsNullOrWhiteSpace(TxtStock.Text) ||
                CmbCategoria.SelectedValue == null)
            {
                ErrorText.Text = "Los campos marcados con * son obligatorios.";
                ErrorPanel.Visibility = Visibility.Visible;
                return;
            }

            if (!int.TryParse(TxtStock.Text, out int stock) || stock < 0)
            {
                ErrorText.Text = "El stock debe ser un numero entero positivo.";
                ErrorPanel.Visibility = Visibility.Visible;
                return;
            }

            try
            {
                await _api.EditarRevistaAsync(_recurso.Id, new AgregarRevistaRequest
                {
                    Titulo = TxtTitulo.Text.Trim(),
                    Autor = TxtAutor.Text.Trim(),
                    CategoriaId = (int)CmbCategoria.SelectedValue,
                    ISSN = TxtISSN.Text.Trim(),
                    NumeroEdicion = int.TryParse(TxtNumeroEdicion.Text, out int num) ? num : 0,
                    FechaPublicacion = DateTime.TryParse(TxtFechaPublicacion.Text, out DateTime fecha) ? fecha : null,
                    Stock = stock,
                    ImagenBytes = _imagenBytes,
                    ImagenNombre = _imagenNombre
                });
                DialogResult = true;
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"Error al guardar: {ex.Message}";
                ErrorPanel.Visibility = Visibility.Visible;
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
            => DialogResult = false;
    }
}
