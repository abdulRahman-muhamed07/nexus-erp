using XeoTechErp.Api.Application.Abstractions;

namespace XeoTechErp.Api.Infrastructure.Persistence;

public sealed class UnitOfWork(XeoTechDbContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}