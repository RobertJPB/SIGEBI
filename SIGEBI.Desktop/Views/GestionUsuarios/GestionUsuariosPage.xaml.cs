using System;
using System.Windows;
using System.Windows.Controls;
using SIGEBI.ViewModels;

namespace SIGEBI.Views.GestionUsuarios
{
    public partial class GestionUsuariosPage : Page
    {
        private readonly SIGEBI.ViewModels.GestionUsuariosViewModel _viewModel;

        public GestionUsuariosPage()
        {
            InitializeComponent();
            _viewModel = (SIGEBI.ViewModels.GestionUsuariosViewModel)SIGEBI.App.Current.Services.GetService(typeof(SIGEBI.ViewModels.GestionUsuariosViewModel))!;
            DataContext = _viewModel;

            Loaded += (s, e) => _ = _viewModel.CargarUsuariosAsync();
        }

        private void BtnNuevoUsuario_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new UsuarioDialog { Owner = Window.GetWindow(this) };
            if (dialog.ShowDialog() == true && dialog.Usuario != null)
            {
                _ = _viewModel.RegistrarUsuarioAsync(dialog.Usuario);
            }
        }
    }
}
