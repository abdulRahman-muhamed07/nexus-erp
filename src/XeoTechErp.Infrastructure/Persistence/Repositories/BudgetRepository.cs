using Microsoft.EntityFrameworkCore;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Infrastructure.Persistence.Repositories;

public sealed class BudgetRepository(XeoTechDbContext db) : IBudgetRepository
{
    public async Task<IReadOnlyList<Budget>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await db.Budgets.AsNoTracking().OrderBy(x => x.Category).ToListAsync(cancellationToken);

    public Task<Budget?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.Budgets.FindAsync([id], cancellationToken).AsTask();

    public Task<Budget?> GetByCategoryAsync(string category, CancellationToken cancellationToken = default) =>
        db.Budgets.FirstOrDefaultAsync(x => x.Category == category, cancellationToken);

    public void Add(Budget budget) => db.Budgets.Add(budget);

    public void Remove(Budget budget) => db.Budgets.Remove(budget);
}