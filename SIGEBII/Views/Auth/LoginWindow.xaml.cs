using System.Windows;
using SIGEBI.Services;
using SIGEBII;

namespace SIGEBI.Views.Auth
{
    public partial class LoginWindow : Window
    {
        private readonly ApiService _apiService;

        public LoginWindow()
        {
            InitializeComponent();
            _apiService = new ApiService();
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var correo = TxtCorreo.Text.Trim();
            var contrasena = TxtContrasena.Password;

            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(contrasena))
            {
                MostrarError("Correo y contraseña son obligatorios.");
                return;
            }

            try
            {
                var token = await _apiService.LoginAsync(correo, contrasena);

                if (string.IsNullOrWhiteSpace(token))
                {
                    MostrarError("Correo o contraseña incorrectos.");
                    return;
                }

                _apiService.SetToken(token);
                SessionService.Token = token;
                SessionService.ApiService = _apiService;

                var mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MostrarError($"No se pudo conectar con el servidor. {ex.Message}");
            }
        }

        private void MostrarError(string mensaje)
        {
            ErrorText.Text = mensaje;
            ErrorPanel.Visibility = Visibility.Visible;
        }
    }
}
