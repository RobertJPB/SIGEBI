using System.Windows;
using SIGEBI.Services;
using SIGEBI.Business.DTOs;

namespace SIGEBI.Views.Auth
{
    public partial class LoginWindow : Window
    {
        private readonly SIGEBI.ViewModels.LoginViewModel _viewModel;

        public LoginWindow()
        {
            InitializeComponent();
            _viewModel = (SIGEBI.ViewModels.LoginViewModel)SIGEBI.App.Current.Services.GetService(typeof(SIGEBI.ViewModels.LoginViewModel))!;
            DataContext = _viewModel;

            _viewModel.OnLoginSuccess = () =>
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            };
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            // El PasswordBox no soporta TwoWay Binding fácilmente en WPF sin plugins externos
            // por seguridad, así que se pasa por parámetro al método del ViewModel directamente.
            var contrasena = TxtContrasena.Password;
            await _viewModel.LoginAsync(contrasena);
        }
    }
}
