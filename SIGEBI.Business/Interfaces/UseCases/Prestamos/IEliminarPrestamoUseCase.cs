namespace SIGEBI.Business.Interfaces.UseCases.Prestamos
{
    /// <summary>
    /// Contrato para la eliminación permanente de un registro de préstamo.
    /// </summary>
    public interface IEliminarPrestamoUseCase
    {
        Task EjecutarAsync(Guid id);
    }
}
