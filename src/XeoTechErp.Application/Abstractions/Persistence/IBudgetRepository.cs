using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Application.Abstractions.Persistence;

public interface IBudgetRepository
{
    Task<IReadOnlyList<Budget>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Budget?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Budget?> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);
    void Add(Budget budget);
    void Remove(Budget budget);
}