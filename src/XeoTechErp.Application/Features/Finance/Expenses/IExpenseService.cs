using XeoTechErp.Application.Common;

namespace XeoTechErp.Application.Features.Finance.Expenses;

public interface IExpenseService
{
    Task<PagedResult<ExpenseResponse>> GetAsync(int page, int pageSize, string? category, CancellationToken cancellationToken = default);
    Task<Result<ExpenseResponse>> CreateAsync(CreateExpenseRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
