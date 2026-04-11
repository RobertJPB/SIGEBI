using System;
namespace SIGEBI.Domain.Enums.Seguridad
{
    // ATENCIÓN: Los valores numéricos (IDs) son CRÍTICOS.
    // No cambiarlos sin actualizar la lógica de mapeo en SIGEBI.Desktop (XAML) y SIGEBI.Web (Controllers).
    public enum RolUsuario
    {
        Administrador = 1,
        Bibliotecario = 2,
        Estudiante = 3,
        Docente = 4
    }
}
