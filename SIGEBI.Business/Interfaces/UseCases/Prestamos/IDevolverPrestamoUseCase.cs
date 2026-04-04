namespace SIGEBI.Business.Interfaces.UseCases.Prestamos
{
    /// <summary>
    /// Contrato para el caso de uso que registra la devolución de un recurso
    /// y calcula penalizaciones por atraso.
    /// </summary>
    public interface IDevolverPrestamoUseCase
    {
        Task EjecutarAsync(Guid prestamoId);
    }
}
