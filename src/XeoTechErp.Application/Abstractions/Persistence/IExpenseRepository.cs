using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Application.Abstractions.Persistence;

public interface IExpenseRepository
{
    Task<IReadOnlyList<Expense>> GetPageAsync(int page, int pageSize, string? category, CancellationToken cancellationToken = default);
    Task<int> CountAsync(string? category, CancellationToken cancellationToken = default);
    Task<Expense?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    void Add(Expense expense);
    void Remove(Expense expense);
}