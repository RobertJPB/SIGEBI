using CommunityToolkit.Mvvm.ComponentModel;

namespace SIGEBII.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        // Propiedad observable para el título cambiante de la sección central
        [ObservableProperty]
        private string _seccionActual = "Gestión Bibliográfica";

        public MainViewModel()
        {
            // Título principal de la ventana (Proof of Concept)
            Title = "SIGEBI MVVM - Panel Administrativo";
        }
    }
}
