using System;
using System.Linq;
using System.Windows;
using SIGEBI.Services;
using SIGEBI.Business.DTOs;

namespace SIGEBI.Views.GestionPrestamos
{
    public partial class AgregarPrestamoDialog : Window
    {
        private readonly ApiService _apiService;

        public AgregarPrestamoDialog()
        {
            InitializeComponent();
            _apiService = (ApiService)Application.Current.Resources["ApiService"] ?? 
                          (ApiService)SIGEBI.App.Current.Services.GetService(typeof(ApiService))!;
            
            Loaded += AgregarPrestamoDialog_Loaded;
        }

        private async void AgregarPrestamoDialog_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var usuarios = await _apiService.GetUsuariosAsync();
                CmbUsuarios.ItemsSource = usuarios.Where(u => u.Estado == 1).ToList();

                var recursos = await _apiService.GetRecursosAsync();
                // Filter resources that have stock available
                CmbRecursos.ItemsSource = recursos.Where(r => r.Stock > 0).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (CmbUsuarios.SelectedValue == null)
            {
                MessageBox.Show("Por favor, seleccione un usuario.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CmbRecursos.SelectedValue == null)
            {
                MessageBox.Show("Por favor, seleccione un recurso.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var request = new PrestamoRequestDTO
                {
                    UsuarioId = (Guid)CmbUsuarios.SelectedValue,
                    RecursoId = (Guid)CmbRecursos.SelectedValue,
                    FechaDevolucionEstimada = DpFechaDevolucion.SelectedDate
                };

                await _apiService.SolicitarPrestamoAsync(request);
                MessageBox.Show("Préstamo registrado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al registrar el préstamo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
