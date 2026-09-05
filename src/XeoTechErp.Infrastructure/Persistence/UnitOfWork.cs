using XeoTechErp.Application.Abstractions.Persistence;

namespace XeoTechErp.Infrastructure.Persistence;

public sealed class UnitOfWork(XeoTechDbContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);

    public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        var strategy = db.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await action();
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }
}
