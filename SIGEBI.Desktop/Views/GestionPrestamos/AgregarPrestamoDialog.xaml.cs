using System;
using System.Linq;
using System.Windows;
using SIGEBI.Services;
using SIGEBI.Business.DTOs;

namespace SIGEBI.Views.GestionPrestamos
{
    public partial class AgregarPrestamoDialog : Window
    {
        private readonly IUsuariosApi _usuariosApi;
        private readonly IRecursosApi _recursosApi;
        private readonly IPrestamosApi _prestamosApi;

        public AgregarPrestamoDialog()
            : this(
                (IUsuariosApi)SIGEBI.App.Current.Services.GetService(typeof(IUsuariosApi))!,
                (IRecursosApi)SIGEBI.App.Current.Services.GetService(typeof(IRecursosApi))!,
                (IPrestamosApi)SIGEBI.App.Current.Services.GetService(typeof(IPrestamosApi))!) { }

        public AgregarPrestamoDialog(IUsuariosApi usuariosApi, IRecursosApi recursosApi, IPrestamosApi prestamosApi)
        {
            InitializeComponent();
            _usuariosApi = usuariosApi;
            _recursosApi = recursosApi;
            _prestamosApi = prestamosApi;
            Loaded += AgregarPrestamoDialog_Loaded;
        }

        private async void AgregarPrestamoDialog_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var usuarios = await _usuariosApi.GetUsuariosAsync();
                CmbUsuarios.ItemsSource = usuarios.Where(u => u.Estado == (int)SIGEBI.Domain.Enums.Seguridad.EstadoUsuario.Activo).ToList();

                var recursos = await _recursosApi.GetRecursosAsync();
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

                await _prestamosApi.SolicitarPrestamoAsync(request);
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
