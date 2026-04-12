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
using System.Linq;

namespace SIGEBI.ViewModels
{
    public partial class ReportesViewModel : BaseViewModel
    {
        private readonly IReportesApi _api;
        private readonly IUsuariosApi _usuariosApi;
        private readonly IRecursosApi _recursosApi;

        [ObservableProperty]
        private ReporteDTO? _dashboard;

        [ObservableProperty]
        private ObservableCollection<PrestamoResponseDTO> _prestamos = new();

        [ObservableProperty]
        private ObservableCollection<dynamic> _usuariosMorosos = new();

        [ObservableProperty]
        private ObservableCollection<PenalizacionDTO> _penalizacionesActivas = new();

        [ObservableProperty]
        private ObservableCollection<UsuarioDTO> _usuariosFiltro = new();

        [ObservableProperty]
        private ObservableCollection<RecursoDetalleDTO> _recursosFiltro = new();

        [ObservableProperty]
        private ObservableCollection<HistorialReporteDTO> _historialReportes = new();

        [ObservableProperty]
        private UsuarioDTO? _usuarioSeleccionado;

        [ObservableProperty]
        private RecursoDetalleDTO? _recursoSeleccionado;

        [ObservableProperty]
        private DateTime _fechaInicio = DateTime.Now.AddMonths(-1);

        [ObservableProperty]
        private DateTime _fechaFin = DateTime.Now;

        [ObservableProperty]
        private bool _hayResultados;

        [ObservableProperty]
        private ObservableCollection<string> _tiposDeReporte = new() 
        { 
            "Préstamos por Período", 
            "Usuarios más penalizados", 
            "Penalizaciones Activas",
            "Reporte General"
        };

        [ObservableProperty]
        private string _tipoReporteSeleccionado = "Préstamos por Período";

        [ObservableProperty]
        private bool _muestraTituloResultados;

        [ObservableProperty]
        private bool _muestraFiltrosPrestamos = true;

        [ObservableProperty]
        private bool _muestraGridPrestamos = true;

        [ObservableProperty]
        private bool _muestraGridMorosos;

        [ObservableProperty]
        private bool _muestraGridPenalizaciones;

        partial void OnTipoReporteSeleccionadoChanged(string value)
        {
            MuestraFiltrosPrestamos = value == "Préstamos por Período";
            HayResultados = false; // Reset results when switching
            MuestraTituloResultados = false;
            
            MuestraGridPrestamos = value == "Préstamos por Período";
            MuestraGridMorosos = value == "Usuarios más penalizados";
            MuestraGridPenalizaciones = value == "Penalizaciones Activas";
        }

        public ReportesViewModel(IReportesApi api, IUsuariosApi usuariosApi, IRecursosApi recursosApi)
        {
            _api = api;
            _usuariosApi = usuariosApi;
            _recursosApi = recursosApi;
            Title = "Reportes y Estadísticas";
            _ = InicializarAsync();
        }

        private async Task InicializarAsync()
        {
            await Task.WhenAll(
                CargarDashboardAsync(),
                CargarMorososAsync(),
                CargarPenalizacionesActivasAsync(),
                CargarCombosFiltroAsync(),
                CargarHistorialAsync()
            );
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

        public async Task CargarCombosFiltroAsync()
        {
            try
            {
                var usuarios = await _usuariosApi.GetUsuariosAsync();
                UsuariosFiltro = new ObservableCollection<UsuarioDTO>(usuarios.OrderBy(u => u.Nombre));

                var recursos = await _recursosApi.GetRecursosAsync();
                RecursosFiltro = new ObservableCollection<RecursoDetalleDTO>(recursos.OrderBy(r => r.Titulo));
            }
            catch { /* Silencioso */ }
        }

        [RelayCommand]
        public async Task CargarHistorialAsync()
        {
            try
            {
                var data = await _api.GetHistorialReportesAsync(10);
                HistorialReportes = new ObservableCollection<HistorialReporteDTO>(data);
            }
            catch { /* Silencioso */ }
        }

        [RelayCommand]
        public async Task GenerarReporteDinamicoAsync()
        {
            try
            {
                IsBusy = true;
                LimpiarError();

                switch (TipoReporteSeleccionado)
                {
                    case "Préstamos por Período":
                        var dataP = await _api.GetPrestamosPorPeriodoAsync(
                            FechaInicio, FechaFin, UsuarioSeleccionado?.Id, RecursoSeleccionado?.Id);
                        Prestamos = new ObservableCollection<PrestamoResponseDTO>(dataP);
                        HayResultados = Prestamos.Count > 0;
                        MuestraTituloResultados = HayResultados;
                        break;
                    case "Usuarios más penalizados":
                        var dataM = await _api.GetUsuariosMasPenalizadosAsync(10);
                        UsuariosMorosos = new ObservableCollection<dynamic>(dataM);
                        HayResultados = UsuariosMorosos.Count > 0;
                        MuestraTituloResultados = HayResultados;
                        break;
                    case "Penalizaciones Activas":
                        var dataA = await _api.GetPenalizacionesActivasAsync();
                        PenalizacionesActivas = new ObservableCollection<PenalizacionDTO>(dataA);
                        HayResultados = PenalizacionesActivas.Count > 0;
                        MuestraTituloResultados = HayResultados;
                        break;
                    case "Reporte General":
                        Dashboard = await _api.GetReporteGeneralAsync();
                        HayResultados = true;
                        MuestraTituloResultados = false;
                        break;
                }
                
                // Recargar historial después de generar uno nuevo
                _ = CargarHistorialAsync();
            }
            catch (Exception ex)
            {
                await ManejarErrorAsync(ex, "generar reporte");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public void LimpiarFiltros()
        {
            UsuarioSeleccionado = null;
            RecursoSeleccionado = null;
            FechaInicio = DateTime.Now.AddMonths(-1);
            FechaFin = DateTime.Now;
        }

        [RelayCommand]
        public async Task ExportarExcelAsync()
        {
            if (!HayResultados) return;

            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Archivo CSV (*.csv)|*.csv",
                    FileName = $"Reporte_{TipoReporteSeleccionado.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmm}.csv",
                    Title = $"Exportar {TipoReporteSeleccionado}"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    IsBusy = true;
                    var csv = new StringBuilder();

                    if (TipoReporteSeleccionado == "Préstamos por Período" && Prestamos != null && Prestamos.Count > 0)
                    {
                        csv.AppendLine("Fecha Inicio;Usuario;Recurso;Fecha Devolución Estimada;Estado");
                        foreach (var p in Prestamos)
                        {
                            csv.AppendLine($"{p.FechaInicio:dd/MM/yyyy HH:mm};{p.NombreUsuario};{p.TituloRecurso};{p.FechaDevolucionEstimada:dd/MM/yyyy};{p.Estado}");
                        }
                    }
                    else if (TipoReporteSeleccionado == "Usuarios más penalizados" && UsuariosMorosos != null && UsuariosMorosos.Count > 0)
                    {
                        csv.AppendLine("Nombre;Sanciones Totales");
                        foreach (var m in UsuariosMorosos)
                        {
                            csv.AppendLine($"{m.Nombre};{m.CantidadSanciones}");
                        }
                    }
                    else if (TipoReporteSeleccionado == "Penalizaciones Activas" && PenalizacionesActivas != null && PenalizacionesActivas.Count > 0)
                    {
                        csv.AppendLine("Fecha Inicio;Usuario;Motivo;Fecha Fin");
                        foreach (var p in PenalizacionesActivas)
                        {
                            csv.AppendLine($"{p.FechaDesde:dd/MM/yyyy HH:mm};{p.NombreUsuario};{p.Motivo};{p.FechaHasta:dd/MM/yyyy}");
                        }
                    }
                    else if (TipoReporteSeleccionado == "Reporte General" && Dashboard != null)
                    {
                        csv.AppendLine("Métrica;Valor");
                        csv.AppendLine($"Préstamos Totales;{Dashboard.TotalPrestamos}");
                        csv.AppendLine($"Usuarios Registrados;{Dashboard.TotalUsuarios}");
                        csv.AppendLine($"Recursos Bibliográficos;{Dashboard.TotalRecursos}");
                        csv.AppendLine($"Penalizaciones Activas;{Dashboard.TotalPenalizaciones}");
                    }
                    else
                    {
                        return; // Sin datos
                    }

                    // Guardar con BOM para que Excel detecte UTF-8 correctamente
                    await File.WriteAllTextAsync(saveFileDialog.FileName, csv.ToString(), Encoding.UTF8);
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
