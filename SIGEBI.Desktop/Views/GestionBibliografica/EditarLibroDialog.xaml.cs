using System;
using System.IO;
using System.Windows;
using SIGEBI.Services;
using SIGEBI.Business.DTOs;

namespace SIGEBI.Views.GestionBibliografica
{
    public partial class EditarLibroDialog : Window
    {
        private readonly ResourceUploadService _api;
        private readonly ICategoriasApi _categoriasApi;
        private readonly IRecursosApi _recursosApi;
        private readonly RecursoDetalleDTO _recurso;
        private byte[]? _imagenBytes;
        private string? _imagenNombre;

        public EditarLibroDialog(RecursoDetalleDTO recurso) : this(
            recurso,
            (ResourceUploadService)SIGEBI.App.Current.Services.GetService(typeof(ResourceUploadService))!,
            (ICategoriasApi)SIGEBI.App.Current.Services.GetService(typeof(ICategoriasApi))!,
            (IRecursosApi)SIGEBI.App.Current.Services.GetService(typeof(IRecursosApi))!) { }

        public EditarLibroDialog(RecursoDetalleDTO recurso, ResourceUploadService api, ICategoriasApi categoriasApi, IRecursosApi recursosApi)
        {
            InitializeComponent();
            _recurso = recurso;
            _api = api;
            _categoriasApi = categoriasApi;
            _recursosApi = recursosApi;
            Loaded += async (s, e) =>
            {
                try
                {
                    var categorias = await _categoriasApi.GetCategoriasAsync();
                    CmbCategoria.ItemsSource = categorias;
                    CmbCategoria.SelectedValue = _recurso.CategoriaId;
                }
                catch
                {
                    ErrorText.Text = "No se pudieron cargar las categorias.";
                    ErrorPanel.Visibility = Visibility.Visible;
                }

                TxtTitulo.Text = recurso.Titulo;
                TxtAutor.Text = recurso.Autor;
                TxtDescripcion.Text = recurso.Descripcion ?? "";
                TxtISBN.Text = recurso.ISBN ?? "";
                TxtEditorial.Text = recurso.Editorial ?? "";
                TxtAnio.Text = recurso.Anio?.ToString() ?? "";
                TxtGenero.Text = recurso.Genero ?? "";
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
                await _api.EditarLibroAsync(_recurso.Id, new AgregarLibroRequest
                {
                    Titulo = TxtTitulo.Text.Trim(),
                    Autor = TxtAutor.Text.Trim(),
                    CategoriaId = (int)CmbCategoria.SelectedValue,
                    Descripcion = string.IsNullOrWhiteSpace(TxtDescripcion.Text) ? null : TxtDescripcion.Text.Trim(),
                    ISBN = TxtISBN.Text.Trim(),
                    Editorial = TxtEditorial.Text.Trim(),
                    Anio = int.TryParse(TxtAnio.Text, out int anio) ? anio : null,
                    Genero = string.IsNullOrWhiteSpace(TxtGenero.Text) ? null : TxtGenero.Text.Trim(),
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
