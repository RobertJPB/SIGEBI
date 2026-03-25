using System;
using System.Windows;
using System.Windows.Controls;
using SIGEBI.ViewModels;

namespace SIGEBI.Views.Notificaciones
{
    public partial class NotificacionesPage : Page
    {
        private readonly NotificacionesViewModel _viewModel;

        public NotificacionesPage()
        {
            InitializeComponent();
            _viewModel = (NotificacionesViewModel)SIGEBI.App.Current.Services.GetService(typeof(NotificacionesViewModel))!;
            DataContext = _viewModel;
            Loaded += (s, e) => _ = _viewModel.CargarNotificacionesAsync();
        }
    }
}
