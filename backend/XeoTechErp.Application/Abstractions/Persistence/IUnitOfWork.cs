namespace XeoTechErp.Application.Abstractions.Persistence;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IAsyncDisposable> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
