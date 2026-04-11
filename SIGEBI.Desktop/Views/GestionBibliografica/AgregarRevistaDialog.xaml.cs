using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using SIGEBI.Services;
using SIGEBI.Business.DTOs;


namespace SIGEBI.Views.GestionBibliografica

{
    public partial class AgregarRevistaDialog : Window
    {
        private readonly ResourceUploadService _api;
        private readonly ICategoriasApi _categoriasApi;
        private readonly IRecursosApi _recursosApi;
        private byte[]? _imagenBytes;
        private string? _imagenNombre;

        public AgregarRevistaDialog() : this(
            (ResourceUploadService)SIGEBI.App.Current.Services.GetService(typeof(ResourceUploadService))!, 
            (ICategoriasApi)SIGEBI.App.Current.Services.GetService(typeof(ICategoriasApi))!,
            (IRecursosApi)SIGEBI.App.Current.Services.GetService(typeof(IRecursosApi))!) { }

        public AgregarRevistaDialog(ResourceUploadService api, ICategoriasApi categoriasApi, IRecursosApi recursosApi)
        {
            InitializeComponent();
            _api = api;
            _categoriasApi = categoriasApi;
            _recursosApi = recursosApi;
            Loaded += async (s, e) =>
            {
                try
                {
                    var autoresDb = await _recursosApi.GetAutoresAsync();
                    var sugerencias = new List<string> { 
                        "Gabriel García Márquez", "Isabel Allende", "Jorge Luis Borges", 
                        "Julio Cortázar", "Mario Vargas Llosa", "Pablo Neruda", 
                        "Gabriela Mistral", "Paulo Coelho", "Miguel de Cervantes", 
                        "J.K. Rowling", "Stephen King" 
                    };

                    foreach (var autor in autoresDb)
                    {
                        if (!sugerencias.Contains(autor)) sugerencias.Add(autor);
                    }
                    CmbAutor.ItemsSource = sugerencias;
                }
                catch
                {
                    CmbAutor.ItemsSource = new[] { "Gabriel García Márquez", "Miguel de Cervantes", "J.K. Rowling" };
                }

                try
                {
                    var editorialesDb = await _recursosApi.GetEditorialesAsync();
                    var sugerenciasEds = new List<string> { 
                        "Planeta", "Santillana", "Alfaguara", 
                        "Anagrama", "Siglo XXI", "Fondo de Cultura Económica",
                        "Penguin Random House", "Oxford University Press", "Pearson"
                    };

                    foreach (var ed in editorialesDb)
                    {
                        if (!sugerenciasEds.Contains(ed)) sugerenciasEds.Add(ed);
                    }
                    CmbEditorial.ItemsSource = sugerenciasEds;
                }
                catch
                {
                    CmbEditorial.ItemsSource = new[] { "Planeta", "Santillana", "Oxford" };
                }

                try
                {
                    var categorias = await _categoriasApi.GetCategoriasAsync();
                    CmbCategoria.ItemsSource = categorias;
                }
                catch
                {
                    ErrorText.Text = "No se pudieron cargar las categorias.";
                    ErrorPanel.Visibility = Visibility.Visible;
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
                await _api.AgregarRevistaAsync(new AgregarRevistaRequest
                {
                    Titulo = TxtTitulo.Text.Trim(),
                    Autor = CmbAutor.Text.Trim(),
                    CategoriaId = (int)CmbCategoria.SelectedValue,
                    Descripcion = string.IsNullOrWhiteSpace(TxtDescripcion.Text) ? null : TxtDescripcion.Text.Trim(),
                    ISSN = TxtISSN.Text.Trim(),
                    NumeroEdicion = int.TryParse(TxtNumeroEdicion.Text, out int num) ? num : 0,
                    Anio = int.TryParse(TxtAnio.Text, out int anio) ? anio : 0,
                    NumeroPaginas = int.TryParse(TxtPaginas.Text, out int paginas) ? paginas : null,
                    Editorial = CmbEditorial.Text.Trim(),
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
