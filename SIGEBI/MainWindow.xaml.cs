using System.Windows;
using System.Windows.Controls;
using SIGEBI.Views.GestionBibliografica;
using SIGEBI.Views.GestionPrestamos;
using SIGEBI.Views.Auditoria;
using SIGEBI.Views.Notificaciones;
using SIGEBI.Views.Penalizaciones;
using SIGEBI.Views.GestionUsuarios;

namespace SIGEBI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = SIGEBII.App.Current.Services.GetService(typeof(SIGEBII.ViewModels.MainViewModel));
            Navegar(new GestionBibliograficaPage(), "Gestión Bibliográfica", BtnBibliografica);
        }

        private void BtnBibliografica_Click(object sender, RoutedEventArgs e)
            => Navegar(new GestionBibliograficaPage(), "Gestión Bibliográfica", BtnBibliografica);

        private void BtnPrestamos_Click(object sender, RoutedEventArgs e)
            => Navegar(new GestionPrestamosPage(), "Gestión de Préstamos", BtnPrestamos);

        private void BtnUsuarios_Click(object sender, RoutedEventArgs e)
            => Navegar(new GestionUsuariosPage(), "Usuarios", BtnUsuarios);

        private void BtnPenalizaciones_Click(object sender, RoutedEventArgs e)
            => Navegar(new PenalizacionesPage(), "Penalizaciones", BtnPenalizaciones);

        private void BtnNotificaciones_Click(object sender, RoutedEventArgs e)
            => Navegar(new NotificacionesPage(), "Notificaciones", BtnNotificaciones);

        private void BtnAuditoria_Click(object sender, RoutedEventArgs e)
            => Navegar(new AuditoriaPage(), "Auditoría", BtnAuditoria);

        private void BtnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            var login = new Views.Auth.LoginWindow();
            login.Show();
            this.Close();
        }

        private void Navegar(Page pagina, string titulo, Button btnActivo)
        {
            MainFrame.Navigate(pagina);
            if (DataContext is SIGEBII.ViewModels.MainViewModel vm)
            {
                vm.SeccionActual = titulo;
            }
            else
            {
                TxtSeccionActual.Text = titulo;
            }
        }
    }
}
