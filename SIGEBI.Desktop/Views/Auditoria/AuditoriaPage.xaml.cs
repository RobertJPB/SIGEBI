using System;
using System.Windows;
using System.Windows.Controls;
using SIGEBI.ViewModels;

namespace SIGEBI.Views.Auditoria
{
    public partial class AuditoriaPage : Page
    {
        private readonly AuditoriaViewModel _viewModel;

        public AuditoriaPage()
        {
            InitializeComponent();
            _viewModel = (AuditoriaViewModel)SIGEBI.App.Current.Services.GetService(typeof(AuditoriaViewModel))!;
            DataContext = _viewModel;
            Loaded += (s, e) => _ = _viewModel.CargarAuditoriasAsync();
        }
    }
}
