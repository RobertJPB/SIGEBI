using System.Windows;

namespace SIGEBI.Views.GestionUsuarios
{
    public partial class MotivoDialog : Window
    {
        public string? Motivo { get; private set; }
        public int? Dias { get; private set; }

        public MotivoDialog(string tituloAction)
        {
            InitializeComponent();
            TitleText.Text = $"Escriba el motivo de {tituloAction.ToLower()} *";

            if (tituloAction.Contains("Suspender", System.StringComparison.OrdinalIgnoreCase))
            {
                HelpText.Text = "Suspensión temporal que impide el acceso por un tiempo determinado. Se aplicará según los días especificados.";
                DaysPanel.Visibility = Visibility.Visible;
            }
            else if (tituloAction.Contains("Bloquear", System.StringComparison.OrdinalIgnoreCase))
            {
                HelpText.Text = "Bloqueo permanente de la cuenta por faltas graves o reincidencia.";
            }
            else if (tituloAction.Contains("Desactivar", System.StringComparison.OrdinalIgnoreCase))
            {
                HelpText.Text = "Desactivación administrativa reversible de forma manual cuando sea necesario.";
            }
        }

        private void BtnConfirmar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtMotivo.Text))
            {
                MessageBox.Show("El motivo es obligatorio.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DaysPanel.Visibility == Visibility.Visible && !string.IsNullOrWhiteSpace(TxtDias.Text))
            {
                if (!int.TryParse(TxtDias.Text, out int result) || result <= 0)
                {
                    MessageBox.Show("Los días de suspensión deben ser un número entero positivo.", "Error de validación", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                Dias = result;
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
