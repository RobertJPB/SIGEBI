using System;
using System.Windows;
using SIGEBI.Business.DTOs;
using SIGEBI.ViewModels;

namespace SIGEBI.Views.Penalizaciones
{
    public partial class AplicarPenalizacionWindow : Window
    {
        public AplicarPenalizacionWindow()
        {
            InitializeComponent();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void BtnAplicar_Click(object sender, RoutedEventArgs e)
        {
            if (!Validar()) return;

            var dto = new AplicarPenalizacionManualDTO
            {
                UsuarioId = Guid.Parse(TxtUsuarioId.Text.Trim()),
                Motivo = TxtMotivo.Text.Trim(),
                DiasPenalizacion = int.Parse(TxtDias.Text.Trim())
            };

            if (!string.IsNullOrWhiteSpace(TxtPrestamoId.Text))
            {
                if (Guid.TryParse(TxtPrestamoId.Text.Trim(), out var prestamoId))
                {
                    dto.PrestamoId = prestamoId;
                }
            }

            if (DataContext is PenalizacionesViewModel vm)
            {
                BtnAplicar.IsEnabled = false;
                await vm.AplicarManualCommand.ExecuteAsync(dto);
                BtnAplicar.IsEnabled = true;
                Close();
            }
        }

        private bool Validar()
        {
            ErrorPanel.Visibility = Visibility.Collapsed;
            string error = string.Empty;

            if (string.IsNullOrWhiteSpace(TxtUsuarioId.Text) || !Guid.TryParse(TxtUsuarioId.Text.Trim(), out _))
                error = "El ID del usuario es obligatorio y debe ser un GUID válido.";
            else if (!string.IsNullOrWhiteSpace(TxtPrestamoId.Text) && !Guid.TryParse(TxtPrestamoId.Text.Trim(), out _))
                error = "El ID del préstamo (si se especifica) debe ser un GUID válido.";
            else if (string.IsNullOrWhiteSpace(TxtMotivo.Text))
                error = "El motivo es obligatorio.";
            else if (string.IsNullOrWhiteSpace(TxtDias.Text) || !int.TryParse(TxtDias.Text.Trim(), out int dias) || dias <= 0)
                error = "Los días de penalización deben ser un número mayor a cero.";

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
