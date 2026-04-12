namespace SIGEBI.Business.Interfaces.UseCases.Prestamos
{
    public interface IEliminarPrestamoUseCase
    {
        Task EjecutarAsync(Guid id);
    }
}

