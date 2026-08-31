using XeoTechErp.Application.Common;
using XeoTechErp.Application.Contracts.Finance;
using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Application.Features.Finance;

public interface IFinanceService
{
    Task<FinanceSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AgingBucketDto>> GetAgingAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BudgetVarianceDto>> GetBudgetVarianceAsync(CancellationToken cancellationToken = default);
    Task<PeriodFinanceSummaryDto> GetPeriodSummaryAsync(DateTime? from, DateTime? to, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AssetResponse>> GetAssetsAsync(CancellationToken cancellationToken = default);
    Task<AssetResponse> CreateAssetAsync(CreateAssetRequest request, CancellationToken cancellationToken = default);
    Task<AssetResponse> DisposeAssetAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DepreciationResponse>> GetDepreciationAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BudgetResponse>> GetBudgetsAsync(CancellationToken cancellationToken = default);
    Task<BudgetResponse> UpsertBudgetAsync(UpsertBudgetRequest request, CancellationToken cancellationToken = default);
    Task DeleteBudgetAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResult<ExpenseResponse>> GetExpensesAsync(int page, int pageSize, string? category, CancellationToken cancellationToken = default);
    Task<ExpenseResponse> CreateExpenseAsync(CreateExpenseRequest request, CancellationToken cancellationToken = default);
    Task DeleteExpenseAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InvoiceResponse>> GetInvoicesAsync(InvoiceStatus? status, CancellationToken cancellationToken = default);
    Task<InvoiceResponse> GetInvoiceAsync(int id, CancellationToken cancellationToken = default);
    Task<InvoiceResponse> CreateInvoiceFromOrderAsync(int orderId, CancellationToken cancellationToken = default);
    Task<InvoiceResponse> PayInvoiceAsync(int id, CancellationToken cancellationToken = default);
}
