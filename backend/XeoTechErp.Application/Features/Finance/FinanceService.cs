using XeoTechErp.Application.Abstractions.Persistence;
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

public sealed class FinanceService(IFinanceRepository repository, IUnitOfWork unitOfWork) : IFinanceService
{
    public Task<FinanceSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default) => repository.GetSummaryAsync(cancellationToken);
    public Task<IReadOnlyList<AgingBucketDto>> GetAgingAsync(CancellationToken cancellationToken = default) => repository.GetAgingAsync(cancellationToken);
    public Task<IReadOnlyList<BudgetVarianceDto>> GetBudgetVarianceAsync(CancellationToken cancellationToken = default) => repository.GetBudgetVarianceAsync(cancellationToken);

    public Task<PeriodFinanceSummaryDto> GetPeriodSummaryAsync(DateTime? from, DateTime? to, CancellationToken cancellationToken = default)
    {
        var end = to ?? DateTime.UtcNow;
        var start = from ?? end.Date.AddDays(-30);
        if (start > end) throw new ArgumentException("from must be before to.");
        return repository.GetPeriodSummaryAsync(start, end, cancellationToken);
    }

    public async Task<IReadOnlyList<Asset>> GetAssetsAsync(CancellationToken cancellationToken = default) => await repository.GetAssetsAsync(cancellationToken);

    public async Task<Asset> CreateAssetAsync(Asset asset, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(asset.Name) || asset.Cost < 0 || asset.Salvage < 0 || asset.Salvage > asset.Cost || asset.UsefulLifeYears <= 0)
            throw new ArgumentException("Invalid asset data.");
        asset.Id = 0;
        repository.AddAsset(asset);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return asset;
    }

    public async Task<Asset> DisposeAssetAsync(int id, CancellationToken cancellationToken = default)
    {
        var asset = await repository.GetAssetAsync(id, cancellationToken) ?? throw new KeyNotFoundException("Asset not found.");
        if (asset.Status == AssetStatus.Disposed) throw new InvalidOperationException("Asset already disposed.");
        asset.Status = AssetStatus.Disposed;
        asset.DisposedOn = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return asset;
    }

    public async Task<IReadOnlyList<object>> GetDepreciationAsync(CancellationToken cancellationToken = default)
    {
        var assets = await repository.GetAssetsAsync(cancellationToken);
        return assets.Where(x => x.Status == AssetStatus.InService)
            .Select(x => (object)new { id = x.Id, name = x.Name, monthly = (x.Cost - x.Salvage) / (x.UsefulLifeYears * 12m), annual = (x.Cost - x.Salvage) / x.UsefulLifeYears }).ToList();
    }

    public Task<IReadOnlyList<Budget>> GetBudgetsAsync(CancellationToken cancellationToken = default) => repository.GetBudgetsAsync(cancellationToken);

    public async Task<Budget> UpsertBudgetAsync(Budget input, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(input.Category) || input.MonthlyAmount < 0) throw new ArgumentException("Invalid budget.");
        var category = input.Category.Trim();
        var budget = await repository.GetBudgetByCategoryAsync(category, cancellationToken);
        if (budget is null)
        {
            input.Id = 0; input.Category = category;
            repository.AddBudget(input); budget = input;
        }
        else budget.MonthlyAmount = input.MonthlyAmount;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return budget;
    }

    public async Task DeleteBudgetAsync(int id, CancellationToken cancellationToken = default)
    {
        var budget = await repository.GetBudgetByCategoryAsync(string.Empty, cancellationToken);
        budget = await unitOfWork.GetByIdAsync<Budget>(id, cancellationToken);
        if (budget is null) throw new KeyNotFoundException("Budget not found.");
        repository.RemoveBudget(budget);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Expense> Data, int Total)> GetExpensesAsync(int page, int pageSize, string? category, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1); pageSize = Math.Clamp(pageSize, 1, 100);
        return (await repository.GetExpensesAsync(page, pageSize, category, cancellationToken), await repository.CountExpensesAsync(category, cancellationToken));
    }

    public async Task<Expense> CreateExpenseAsync(Expense expense, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(expense.Category) || expense.Amount <= 0) throw new ArgumentException("Category and a positive amount are required.");
        expense.Id = 0; expense.Category = expense.Category.Trim(); expense.Description = expense.Description?.Trim() ?? string.Empty;
        if (expense.Date == default) expense.Date = DateTime.UtcNow;
        repository.AddExpense(expense); await unitOfWork.SaveChangesAsync(cancellationToken); return expense;
    }

    public async Task DeleteExpenseAsync(int id, CancellationToken cancellationToken = default)
    {
        var expense = await unitOfWork.GetByIdAsync<Expense>(id, cancellationToken) ?? throw new KeyNotFoundException("Expense not found.");
        repository.RemoveExpense(expense); await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public Task<IReadOnlyList<Invoice>> GetInvoicesAsync(InvoiceStatus? status, CancellationToken cancellationToken = default) => repository.GetInvoicesAsync(status, cancellationToken);

    public async Task<Invoice> GetInvoiceAsync(int id, CancellationToken cancellationToken = default) => await repository.GetInvoiceAsync(id, cancellationToken) ?? throw new KeyNotFoundException("Invoice not found.");

    public async Task<Invoice> CreateInvoiceFromOrderAsync(int orderId, CancellationToken cancellationToken = default)
    {
        var order = await repository.GetDeliveredOrderAsync(orderId, cancellationToken) ?? throw new KeyNotFoundException("Order not found.");
        if (order.Status != OrderStatus.Delivered) throw new InvalidOperationException("Invoice can only be created for delivered orders.");
        if (await repository.InvoiceExistsForOrderAsync(orderId, cancellationToken)) throw new InvalidOperationException("Invoice already exists.");
        var days = order.Customer.PaymentTerms switch { "Due on Receipt" => 0, "Net 15" => 15, "Net 45" => 45, "Net 60" => 60, _ => 30 };
        var issued = DateTime.UtcNow;
        var invoice = new Invoice { OrderId = order.Id, CustomerId = order.CustomerId, CustomerName = order.Customer.Company, Amount = order.Total, Issued = issued, Due = issued.AddDays(days) };
        repository.AddInvoice(invoice); await unitOfWork.SaveChangesAsync(cancellationToken); return invoice;
    }

    public async Task<Invoice> PayInvoiceAsync(int id, CancellationToken cancellationToken = default)
    {
        var invoice = await repository.GetInvoiceAsync(id, cancellationToken) ?? throw new KeyNotFoundException("Invoice not found.");
        if (invoice.Status == InvoiceStatus.Paid) return invoice;
        invoice.Status = InvoiceStatus.Paid; invoice.PaidOn = DateTime.UtcNow;
        if (invoice.OrderId is int orderId)
        {
            var paid = await repository.GetOrderPaymentsAsync(orderId, cancellationToken);
            if (paid < invoice.Amount) repository.AddPayment(new Payment { OrderId = orderId, Amount = invoice.Amount - paid, Method = PaymentMethod.Other });
        }
        await unitOfWork.SaveChangesAsync(cancellationToken); return invoice;
    }
}
