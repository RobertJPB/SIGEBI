using System.Windows;
using System.Windows.Controls;
using SIGEBI.Business.DTOs;

namespace SIGEBI.Views.GestionUsuarios
{
    public partial class UsuarioDialog : Window
    {
        public UsuarioDTO? Usuario { get; private set; }

        public UsuarioDialog()
        {
            InitializeComponent();
            // asegurar que empiece vacio
            TxtPass.Text = string.Empty;
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!Validar()) return;

            int rolId = 1;
            if (CmbRol.SelectedItem is ComboBoxItem selectedItem && int.TryParse(selectedItem.Tag.ToString(), out int id))
                rolId = id;

            Usuario = new UsuarioDTO
            {
                Nombre = TxtNombre.Text.Trim(),
                Correo = TxtCorreo.Text.Trim(),
                Contrasena = TxtPass.Text,
                IdRol = rolId
            };

            DialogResult = true;
            Close();
        }

        private bool Validar()
        {
            ErrorPanel.Visibility = Visibility.Collapsed;
            string error = string.Empty;

            if (string.IsNullOrWhiteSpace(TxtNombre.Text)) error = "El nombre es obligatorio.";
            else if (string.IsNullOrWhiteSpace(TxtCorreo.Text)) error = "El correo es obligatorio.";
            else if (!TxtCorreo.Text.Contains("@")) error = "El correo no es válido.";
            else if (string.IsNullOrWhiteSpace(TxtPass.Text)) error = "La contraseña es obligatoria.";
            else if (CmbRol.SelectedIndex == -1) error = "Debe seleccionar un rol.";

            if (!string.IsNullOrEmpty(error))
            {
                ErrorText.Text = error;
                ErrorPanel.Visibility = Visibility.Visible;
                return false;
            }

            return true;
        }
    }
}
