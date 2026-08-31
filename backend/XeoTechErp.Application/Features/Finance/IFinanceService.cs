using XeoTechErp.Domain.Entities;
using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Application.Features.Finance;

public interface IFinanceService
{
    Task<FinanceSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AgingBucketDto>> GetAgingAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BudgetVarianceDto>> GetBudgetVarianceAsync(CancellationToken cancellationToken = default);
    Task<PeriodFinanceSummaryDto> GetPeriodSummaryAsync(DateTime? from, DateTime? to, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Asset>> GetAssetsAsync(CancellationToken cancellationToken = default);
    Task<Asset> CreateAssetAsync(Asset asset, CancellationToken cancellationToken = default);
    Task<Asset> DisposeAssetAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<object>> GetDepreciationAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Budget>> GetBudgetsAsync(CancellationToken cancellationToken = default);
    Task<Budget> UpsertBudgetAsync(Budget input, CancellationToken cancellationToken = default);
    Task DeleteBudgetAsync(int id, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Expense> Data, int Total)> GetExpensesAsync(int page, int pageSize, string? category, CancellationToken cancellationToken = default);
    Task<Expense> CreateExpenseAsync(Expense expense, CancellationToken cancellationToken = default);
    Task DeleteExpenseAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Invoice>> GetInvoicesAsync(InvoiceStatus? status, CancellationToken cancellationToken = default);
    Task<Invoice> GetInvoiceAsync(int id, CancellationToken cancellationToken = default);
    Task<Invoice> CreateInvoiceFromOrderAsync(int orderId, CancellationToken cancellationToken = default);
    Task<Invoice> PayInvoiceAsync(int id, CancellationToken cancellationToken = default);
}
