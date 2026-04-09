using System;
using System.Collections.Generic;
using System.Windows;
using SIGEBI.Business.DTOs;
using SIGEBI.ViewModels;

namespace SIGEBI.Views.Penalizaciones
{
    public partial class AplicarPenalizacionDialog : Window
    {
        private readonly PenalizacionesViewModel _viewModel;

        public AplicarPenalizacionDialog(PenalizacionesViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            CmbUsuarios.ItemsSource = _viewModel.Usuarios;
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private async void BtnAplicar_Click(object sender, RoutedEventArgs e)
        {
            if (CmbUsuarios.SelectedItem is not UsuarioDTO usuario)
            {
                MessageBox.Show("Por favor, seleccione un usuario.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtMotivo.Text))
            {
                MessageBox.Show("Por favor, ingrese el motivo de la sanción.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _viewModel.SelectedUsuario = usuario;
                _viewModel.MotivoManual = TxtMotivo.Text;
                _viewModel.DiasManual = (int)SldDias.Value;

                await _viewModel.EjecutarAplicarManualCommand.ExecuteAsync(null);
                
                if (string.IsNullOrEmpty(_viewModel.MensajeError))
                {
                    DialogResult = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al aplicar la penalización: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
