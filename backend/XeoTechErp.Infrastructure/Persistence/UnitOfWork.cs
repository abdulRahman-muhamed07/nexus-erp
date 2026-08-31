using Microsoft.EntityFrameworkCore.Storage;
using XeoTechErp.Application.Abstractions.Persistence;

namespace XeoTechErp.Infrastructure.Persistence;

public sealed class UnitOfWork(XeoTechDbContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => db.SaveChangesAsync(cancellationToken);

    public async Task<IAsyncDisposable> BeginTransactionAsync(CancellationToken cancellationToken = default)
        => await db.Database.BeginTransactionAsync(cancellationToken);
}
