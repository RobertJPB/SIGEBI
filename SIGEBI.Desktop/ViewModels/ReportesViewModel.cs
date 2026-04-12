using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SIGEBI.Services;
using SIGEBI.Business.DTOs;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Win32;

namespace SIGEBI.ViewModels
{
    public partial class ReportesViewModel : BaseViewModel
    {
        private readonly IReportesApi _api;

        [ObservableProperty]
        private ReporteDTO? _dashboard;

        [ObservableProperty]
        private ObservableCollection<PrestamoResponseDTO> _prestamos = new();

        [ObservableProperty]
        private ObservableCollection<dynamic> _usuariosMorosos = new();

        [ObservableProperty]
        private ObservableCollection<PenalizacionDTO> _penalizacionesActivas = new();

        [ObservableProperty]
        private DateTime _fechaInicio = DateTime.Now.AddMonths(-1);

        [ObservableProperty]
        private DateTime _fechaFin = DateTime.Now;

        [ObservableProperty]
        private bool _hayResultados;

        public ReportesViewModel(IReportesApi api)
        {
            _api = api;
            Title = "Reportes y Estadísticas";
            _ = InicializarAsync();
        }

        private async Task InicializarAsync()
        {
            await CargarDashboardAsync();
            await CargarMorososAsync();
            await CargarPenalizacionesActivasAsync();
        }

        [RelayCommand]
        public async Task CargarDashboardAsync()
        {
            try
            {
                IsBusy = true;
                Dashboard = await _api.GetReporteGeneralAsync();
            }
            catch (Exception ex)
            {
                await ManejarErrorAsync(ex, "cargar estadísticas generales");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task CargarMorososAsync()
        {
            try
            {
                var data = await _api.GetUsuariosMasPenalizadosAsync(5);
                UsuariosMorosos = new ObservableCollection<dynamic>(data);
            }
            catch { /* Silencioso */ }
        }

        [RelayCommand]
        public async Task CargarPenalizacionesActivasAsync()
        {
            try
            {
                var data = await _api.GetPenalizacionesActivasAsync();
                PenalizacionesActivas = new ObservableCollection<PenalizacionDTO>(data);
            }
            catch { /* Silencioso */ }
        }

        [RelayCommand]
        public async Task GenerarReportePrestamosAsync()
        {
            try
            {
                IsBusy = true;
                LimpiarError();
                
                var data = await _api.GetPrestamosPorPeriodoAsync(FechaInicio, FechaFin);
                Prestamos = new ObservableCollection<PrestamoResponseDTO>(data);
                HayResultados = Prestamos.Count > 0;
                
                if (!HayResultados)
                {
                    // Podemos mostrar un aviso opcional
                }
            }
            catch (Exception ex)
            {
                await ManejarErrorAsync(ex, "generar reporte de préstamos");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task ExportarExcelAsync()
        {
            if (Prestamos == null || Prestamos.Count == 0) return;

            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Archivo CSV (*.csv)|*.csv",
                    FileName = $"Reporte_Prestamos_{DateTime.Now:yyyyMMdd_HHmm}.csv",
                    Title = "Exportar Reporte de Préstamos"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    IsBusy = true;
                    var csv = new StringBuilder();
                    
                    // Encabezados
                    csv.AppendLine("Fecha Inicio;Usuario;Recurso;Fecha Devolución Estimada;Estado");

                    foreach (var p in Prestamos)
                    {
                        var linea = $"{p.FechaInicio:dd/MM/yyyy HH:mm};{p.NombreUsuario};{p.TituloRecurso};{p.FechaDevolucionEstimada:dd/MM/yyyy};{p.Estado}";
                        csv.AppendLine(linea);
                    }

                    // Guardar con BOM para que Excel detecte UTF-8 correctamente
                    await File.WriteAllTextAsync(saveFileDialog.FileName, csv.ToString(), Encoding.UTF8);
                    
                    // Opcional: Notificar éxito
                }
            }
            catch (Exception ex)
            {
                await ManejarErrorAsync(ex, "exportar a excel");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
