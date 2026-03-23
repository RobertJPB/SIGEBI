using System;
using System.IO;
using System.Windows;
using SIGEBI.Services;

namespace SIGEBI.Views.GestionBibliografica
{
    public partial class EditarDocumentoDialog : Window
    {
        private readonly ApiService _api;
        private readonly RecursoDetalleDTO _recurso;
        private byte[]? _imagenBytes;
        private string? _imagenNombre;

        public EditarDocumentoDialog() : this((ApiService)SIGEBII.App.Current.Services.GetService(typeof(ApiService))!, new RecursoDetalleDTO()) { }

        public EditarDocumentoDialog(ApiService api, RecursoDetalleDTO recurso)
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
                TxtFormato.Text = recurso.Formato ?? "";
                TxtInstitucion.Text = recurso.Institucion ?? "";
                TxtAnio.Text = recurso.Anio?.ToString() ?? "";
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
                string.IsNullOrWhiteSpace(TxtAutor.Text) ||
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
                await _api.EditarDocumentoAsync(_recurso.Id, new AgregarDocumentoRequest
                {
                    Titulo = TxtTitulo.Text.Trim(),
                    Autor = TxtAutor.Text.Trim(),
                    CategoriaId = (int)CmbCategoria.SelectedValue,
                    Formato = TxtFormato.Text.Trim(),
                    Institucion = TxtInstitucion.Text.Trim(),
                    Anio = int.TryParse(TxtAnio.Text, out int anio) ? anio : null,
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
