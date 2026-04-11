using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using SIGEBI.Business.DTOs;
using SIGEBI.Services;


namespace SIGEBI.Views.GestionBibliografica
{
    public partial class AgregarLibroDialog : Window
    {
        private readonly ResourceUploadService _api;
        private readonly IRecursosApi _recursosApi; 
        private readonly ICategoriasApi _categoriasApi;
        private byte[]? _imagenBytes;
        private string? _imagenNombre;

        public AgregarLibroDialog(ResourceUploadService api, ICategoriasApi categoriasApi, IRecursosApi recursosApi)
        {
            InitializeComponent();
            _api = api;
            _recursosApi = recursosApi;
            _categoriasApi = categoriasApi;

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
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Archivos de Imagen (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _imagenNombre = Path.GetFileName(openFileDialog.FileName);
                _imagenBytes = File.ReadAllBytes(openFileDialog.FileName);
                TxtRutaImagen.Text = openFileDialog.FileName;

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(openFileDialog.FileName);
                bitmap.EndInit();
                ImgPreview.Source = bitmap;
            }
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtTitulo.Text) || 
                string.IsNullOrWhiteSpace(CmbAutor.Text) ||
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
                await _api.AgregarLibroAsync(new AgregarLibroRequest
                {
                    Titulo = TxtTitulo.Text.Trim(),
                    Autor = CmbAutor.Text.Trim(),
                    CategoriaId = (int)CmbCategoria.SelectedValue,
                    Descripcion = string.IsNullOrWhiteSpace(TxtDescripcion.Text) ? null : TxtDescripcion.Text.Trim(),
                    ISBN = TxtISBN.Text.Trim(),
                    Editorial = CmbEditorial.Text.Trim(),
                    Anio = int.TryParse(TxtAnio.Text, out int anio) ? anio : null,
                    NumeroPaginas = int.TryParse(TxtPaginas.Text, out int paginas) ? paginas : null,
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
        {
            DialogResult = false;
        }
    }
}
