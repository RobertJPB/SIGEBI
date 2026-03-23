using CommunityToolkit.Mvvm.ComponentModel;

namespace SIGEBII.ViewModels
{
    // Clase base para todos los ViewModels del sistema.
    // ObservableObject ya implementa INotifyPropertyChanged.
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _title = string.Empty;
    }
}
