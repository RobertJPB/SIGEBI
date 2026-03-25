using System;
using System.Windows;
using System.Windows.Controls;
using SIGEBI.ViewModels;

namespace SIGEBI.Views.GestionPrestamos
{
    public partial class GestionPrestamosPage : Page
    {
        private readonly GestionPrestamosViewModel _viewModel;

        public GestionPrestamosPage()
        {
            InitializeComponent();
            _viewModel = (GestionPrestamosViewModel)SIGEBI.App.Current.Services.GetService(typeof(GestionPrestamosViewModel))!;
            DataContext = _viewModel;
            Loaded += (s, e) => _ = _viewModel.CargarPrestamosAsync();
        }

        private async void BtnRegistrarPrestamo_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AgregarPrestamoDialog();
            dialog.Owner = Window.GetWindow(this);
            if (dialog.ShowDialog() == true)
            {
                await _viewModel.CargarPrestamosAsync();
            }
        }

    }
}
