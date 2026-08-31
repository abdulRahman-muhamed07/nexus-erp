using XeoTechErp.Application.Features.Finance.Assets;
using XeoTechErp.Application.Features.Finance.Budgets;
using XeoTechErp.Application.Features.Finance.Dashboard;
using XeoTechErp.Application.Features.Finance.Expenses;
using XeoTechErp.Application.Features.Finance.Invoices;
using XeoTechErp.Domain.Entities;
using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Application.Abstractions.Persistence;

public interface IFinanceRepository
{
    Task<FinanceSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AgingBucketDto>> GetAgingAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BudgetVarianceDto>> GetBudgetVarianceAsync(CancellationToken cancellationToken = default);
    Task<PeriodFinanceSummaryDto> GetPeriodSummaryAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Asset>> GetAssetsAsync(CancellationToken cancellationToken = default);
    Task<Asset?> GetAssetAsync(int id, CancellationToken cancellationToken = default);
    void AddAsset(Asset asset);
    Task<IReadOnlyList<Budget>> GetBudgetsAsync(CancellationToken cancellationToken = default);
    Task<Budget?> GetBudgetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Budget?> GetBudgetByCategoryAsync(string category, CancellationToken cancellationToken = default);
    void AddBudget(Budget budget);
    void RemoveBudget(Budget budget);
    Task<IReadOnlyList<Expense>> GetExpensesAsync(int page, int pageSize, string? category, CancellationToken cancellationToken = default);
    Task<int> CountExpensesAsync(string? category, CancellationToken cancellationToken = default);
    Task<Expense?> GetExpenseAsync(int id, CancellationToken cancellationToken = default);
    void AddExpense(Expense expense);
    void RemoveExpense(Expense expense);
    Task<IReadOnlyList<Invoice>> GetInvoicesAsync(InvoiceStatus? status, CancellationToken cancellationToken = default);
    Task<Invoice?> GetInvoiceAsync(int id, CancellationToken cancellationToken = default);
    Task<Order?> GetDeliveredOrderAsync(int orderId, CancellationToken cancellationToken = default);
    Task<bool> InvoiceExistsForOrderAsync(int orderId, CancellationToken cancellationToken = default);
    Task<decimal> GetOrderPaymentsAsync(int orderId, CancellationToken cancellationToken = default);
    void AddInvoice(Invoice invoice);
    void AddPayment(Payment payment);
}
