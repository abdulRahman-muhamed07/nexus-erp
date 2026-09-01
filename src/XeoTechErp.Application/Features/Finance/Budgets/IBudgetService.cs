using XeoTechErp.Application.Common;

namespace XeoTechErp.Application.Features.Finance.Budgets;

public interface IBudgetService
{
    Task<IReadOnlyList<BudgetResponse>> GetAsync(CancellationToken cancellationToken = default);
    Task<Result<BudgetResponse>> UpsertAsync(UpsertBudgetRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
