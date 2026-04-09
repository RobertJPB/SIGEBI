using System;
using System.Windows;
using System.Windows.Controls;
using SIGEBI.ViewModels;

namespace SIGEBI.Views.Penalizaciones
{
    public partial class PenalizacionesPage : Page
    {
        private readonly PenalizacionesViewModel _viewModel;

        public PenalizacionesPage()
        {
            InitializeComponent();
            _viewModel = (PenalizacionesViewModel)SIGEBI.App.Current.Services.GetService(typeof(PenalizacionesViewModel))!;
            DataContext = _viewModel;
            Loaded += (s, e) => _ = _viewModel.CargarPenalizacionesAsync();
        }

        private void BtnAplicarPenalizacion_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AplicarPenalizacionDialog(_viewModel)
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() == true)
            {
                // El ViewModel ya maneja la carga después de aplicar
            }
        }
    }
}
