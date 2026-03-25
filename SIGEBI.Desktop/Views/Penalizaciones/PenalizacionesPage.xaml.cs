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
    }
}
