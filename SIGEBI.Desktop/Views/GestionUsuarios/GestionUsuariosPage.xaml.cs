using System;
using System.Windows;
using System.Windows.Controls;
using SIGEBI.ViewModels;

namespace SIGEBI.Views.GestionUsuarios
{
    public partial class GestionUsuariosPage : Page
    {
        private readonly GestionUsuariosViewModel _viewModel;

        public GestionUsuariosPage()
        {
            InitializeComponent();
            _viewModel = (GestionUsuariosViewModel)((global::SIGEBI.App)Application.Current).Services.GetService(typeof(GestionUsuariosViewModel))!;
            DataContext = _viewModel;
            Loaded += (s, e) => _ = _viewModel.CargarUsuariosAsync();
        }
    }
}
