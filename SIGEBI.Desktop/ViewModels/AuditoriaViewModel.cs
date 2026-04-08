using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SIGEBI.Services;
using SIGEBI.Business.DTOs;

namespace SIGEBI.ViewModels
{
    public partial class AuditoriaViewModel : BaseViewModel
    {
        private readonly ISigebiApi _api;
        private List<AuditoriaDTO> _allAuditorias = new();

        [ObservableProperty] private ObservableCollection<AuditoriaDTO> _auditorias = new();
        [ObservableProperty] private string _contador = "0 registros";
        [ObservableProperty] private string _filtroUsuario = string.Empty;
        [ObservableProperty] private DateTime? _filtroFecha;

        public AuditoriaViewModel(ISigebiApi api)
        {
            _api = api;
            Title = "Auditoría del Sistema";
        }

        partial void OnFiltroUsuarioChanged(string value) => AplicarFiltros();
        partial void OnFiltroFechaChanged(DateTime? value) => AplicarFiltros();

        private void AplicarFiltros()
        {
            var resultado = _allAuditorias.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(FiltroUsuario))
                resultado = resultado.Where(a => a.NombreUsuario != null &&
                    a.NombreUsuario.Contains(FiltroUsuario, StringComparison.OrdinalIgnoreCase));

            if (FiltroFecha.HasValue)
                resultado = resultado.Where(a => a.FechaRegistro.Date == FiltroFecha.Value.Date);

            Auditorias = new ObservableCollection<AuditoriaDTO>(resultado);
            Contador = $"{Auditorias.Count} registros";
        }

        [RelayCommand]
        public async Task CargarAuditoriasAsync()
        {
            try
            {
                IsBusy = true;
                LimpiarError();
                var data = await _api.GetAuditoriasAsync();
                _allAuditorias = data;
                AplicarFiltros();
            }
            catch (Exception ex)
            {
                await ManejarErrorAsync(ex, "cargar logs de auditoría");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public void LimpiarFiltros()
        {
            FiltroUsuario = string.Empty;
            FiltroFecha = null;
        }
    }
}
