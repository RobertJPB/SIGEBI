using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
        private Button _activeBtn;
        private readonly SolidColorBrush _activeBg = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1a5c2e"));
        private readonly SolidColorBrush _inactiveBg = Brushes.Transparent;
        private readonly SolidColorBrush _activeFg = Brushes.White;
        private readonly SolidColorBrush _inactiveFg = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#a0c4ac"));
        private readonly SolidColorBrush _activeIndicator = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4caf7d"));
        private readonly SolidColorBrush _inactiveIndicator = Brushes.Transparent;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = SIGEBI.App.Current.Services.GetService(typeof(SIGEBI.ViewModels.MainViewModel));
            _activeBtn = BtnBibliografica;
            Navegar(new GestionBibliograficaPage(), "Gestión Bibliográfica", BtnBibliografica, TxtNavBibliografica, BrdBibliografica);
        }

        private void BtnBibliografica_Click(object sender, RoutedEventArgs e)
            => Navegar(new GestionBibliograficaPage(), "Gestión Bibliográfica", BtnBibliografica, TxtNavBibliografica, BrdBibliografica);

        private void BtnPrestamos_Click(object sender, RoutedEventArgs e)
            => Navegar(new GestionPrestamosPage(), "Gestión de Préstamos", BtnPrestamos, TxtNavPrestamos, BrdPrestamos);

        private void BtnUsuarios_Click(object sender, RoutedEventArgs e)
            => Navegar(new GestionUsuariosPage(), "Usuarios", BtnUsuarios, TxtNavUsuarios, BrdUsuarios);

        private void BtnPenalizaciones_Click(object sender, RoutedEventArgs e)
            => Navegar(new PenalizacionesPage(), "Penalizaciones", BtnPenalizaciones, TxtNavPenalizaciones, BrdPenalizaciones);

        private void BtnNotificaciones_Click(object sender, RoutedEventArgs e)
            => Navegar(new NotificacionesPage(), "Notificaciones", BtnNotificaciones, TxtNavNotificaciones, BrdNotificaciones);

        private void BtnAuditoria_Click(object sender, RoutedEventArgs e)
            => Navegar(new AuditoriaPage(), "Auditoría", BtnAuditoria, TxtNavAuditoria, BrdAuditoria);

        private void BtnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            var login = new Views.Auth.LoginWindow();
            login.Show();
            this.Close();
        }

        private void Navegar(Page pagina, string titulo, Button btnActivo, TextBlock txtActivo, Border brdActivo)
        {
            ResetNavState();
            
            // Set new active state
            _activeBtn = btnActivo;
            _activeBtn.Style = (Style)FindResource("NavBtnActive");
            txtActivo.Foreground = _activeFg;
            brdActivo.Background = _activeIndicator;

            MainFrame.Navigate(pagina);
            if (DataContext is SIGEBI.ViewModels.MainViewModel vm)
            {
                vm.SeccionActual = titulo;
            }
            else
            {
                TxtSeccionActual.Text = titulo;
            }
        }

        private void ResetNavState()
        {
            var btnStyle = (Style)FindResource("NavBtn");
            
            BtnBibliografica.Style = btnStyle;
            TxtNavBibliografica.Foreground = _inactiveFg;
            BrdBibliografica.Background = _inactiveIndicator;

            BtnPrestamos.Style = btnStyle;
            TxtNavPrestamos.Foreground = _inactiveFg;
            BrdPrestamos.Background = _inactiveIndicator;

            BtnUsuarios.Style = btnStyle;
            TxtNavUsuarios.Foreground = _inactiveFg;
            BrdUsuarios.Background = _inactiveIndicator;

            BtnPenalizaciones.Style = btnStyle;
            TxtNavPenalizaciones.Foreground = _inactiveFg;
            BrdPenalizaciones.Background = _inactiveIndicator;

            BtnNotificaciones.Style = btnStyle;
            TxtNavNotificaciones.Foreground = _inactiveFg;
            BrdNotificaciones.Background = _inactiveIndicator;

            BtnAuditoria.Style = btnStyle;
            TxtNavAuditoria.Foreground = _inactiveFg;
            BrdAuditoria.Background = _inactiveIndicator;
        }
    }
}
