using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using SIGEBI.ViewModels;

namespace SIGEBI.Views.Reportes
{
    public partial class ReportesPage : Page
    {
        public ReportesPage()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService<ReportesViewModel>();
        }
    }
}
