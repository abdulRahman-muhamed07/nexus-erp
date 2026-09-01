using Microsoft.EntityFrameworkCore;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Infrastructure.Persistence.Repositories;

public sealed class ExpenseRepository(XeoTechDbContext db) : IExpenseRepository
{
    public async Task<IReadOnlyList<Expense>> GetPageAsync(int page, int pageSize, string? category, CancellationToken cancellationToken = default)
    {
        var query = db.Expenses.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(x => x.Category == category);

        return await query.OrderByDescending(x => x.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(string? category, CancellationToken cancellationToken = default)
    {
        var query = db.Expenses.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(x => x.Category == category);

        return await query.CountAsync(cancellationToken);
    }

    public Task<Expense?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.Expenses.FindAsync([id], cancellationToken).AsTask();

    public void Add(Expense expense) => db.Expenses.Add(expense);

    public void Remove(Expense expense) => db.Expenses.Remove(expense);
}