using Microsoft.EntityFrameworkCore;
using XeoTechErp.Application.Abstractions.Persistence;

namespace XeoTechErp.Infrastructure.Persistence.Repositories;

public class EfRepository<TEntity>(XeoTechDbContext db) : IRepository<TEntity> where TEntity : class
{
    protected DbSet<TEntity> Set => db.Set<TEntity>();
    public virtual Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default) => Set.FindAsync([id], cancellationToken).AsTask();
    public void Add(TEntity entity) => Set.Add(entity);
    public void Remove(TEntity entity) => Set.Remove(entity);
}
