using XeoTechErp.Application.Abstractions.Persistence;
namespace XeoTechErp.Infrastructure.Persistence;
public sealed class UnitOfWork(XeoTechDbContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => db.SaveChangesAsync(cancellationToken);
}
