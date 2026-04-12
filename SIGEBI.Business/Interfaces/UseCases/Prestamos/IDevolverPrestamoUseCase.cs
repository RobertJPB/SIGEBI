namespace SIGEBI.Business.Interfaces.UseCases.Prestamos
{
    public interface IDevolverPrestamoUseCase
    {
        Task EjecutarAsync(Guid prestamoId);
    }
}

