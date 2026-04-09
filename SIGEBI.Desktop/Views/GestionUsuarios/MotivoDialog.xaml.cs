using System.Windows;

namespace SIGEBI.Views.GestionUsuarios
{
    public partial class MotivoDialog : Window
    {
        public string? Motivo { get; private set; }

        public MotivoDialog(string tituloAction)
        {
            InitializeComponent();
            TitleText.Text = $"Escriba el motivo de {tituloAction.ToLower()} *";
        }

        private void BtnConfirmar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtMotivo.Text))
            {
                MessageBox.Show("El motivo es obligatorio.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Motivo = TxtMotivo.Text;
            DialogResult = true;
            Close();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
