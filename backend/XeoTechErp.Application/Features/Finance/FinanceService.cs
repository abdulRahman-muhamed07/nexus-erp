using AutoMapper;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Application.Common;
using XeoTechErp.Application.Contracts.Finance;
using XeoTechErp.Domain.Entities;
using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Application.Features.Finance;

public sealed class FinanceService(IFinanceRepository repository, IUnitOfWork unitOfWork, IMapper mapper) : IFinanceService
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

    public async Task<IReadOnlyList<AssetResponse>> GetAssetsAsync(CancellationToken cancellationToken = default)
        => mapper.Map<IReadOnlyList<AssetResponse>>(await repository.GetAssetsAsync(cancellationToken));

    public async Task<AssetResponse> CreateAssetAsync(CreateAssetRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || request.Cost < 0 || request.Salvage < 0 || request.Salvage > request.Cost || request.UsefulLifeYears <= 0)
            throw new ArgumentException("Invalid asset data.");

        var asset = new Asset
        {
            Name = request.Name.Trim(),
            Category = request.Category?.Trim() ?? string.Empty,
            PurchaseDate = request.PurchaseDate == default ? DateTime.UtcNow : request.PurchaseDate,
            Cost = request.Cost,
            Salvage = request.Salvage,
            UsefulLifeYears = request.UsefulLifeYears,
            Status = AssetStatus.InService
        };

        repository.AddAsset(asset);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return mapper.Map<AssetResponse>(asset);
    }

    public async Task<AssetResponse> DisposeAssetAsync(int id, CancellationToken cancellationToken = default)
    {
        var asset = await repository.GetAssetAsync(id, cancellationToken) ?? throw new KeyNotFoundException("Asset not found.");
        if (asset.Status == AssetStatus.Disposed) throw new InvalidOperationException("Asset already disposed.");
        asset.Status = AssetStatus.Disposed;
        asset.DisposedOn = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return mapper.Map<AssetResponse>(asset);
    }

    public async Task<IReadOnlyList<DepreciationResponse>> GetDepreciationAsync(CancellationToken cancellationToken = default)
    {
        var assets = await repository.GetAssetsAsync(cancellationToken);
        return assets.Where(x => x.Status == AssetStatus.InService)
            .Select(x => new DepreciationResponse(x.Id, x.Name,
                Math.Round((x.Cost - x.Salvage) / (x.UsefulLifeYears * 12m), 2),
                Math.Round((x.Cost - x.Salvage) / x.UsefulLifeYears, 2)))
            .ToList();
    }

    public async Task<IReadOnlyList<BudgetResponse>> GetBudgetsAsync(CancellationToken cancellationToken = default)
        => mapper.Map<IReadOnlyList<BudgetResponse>>(await repository.GetBudgetsAsync(cancellationToken));

    public async Task<BudgetResponse> UpsertBudgetAsync(UpsertBudgetRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Category) || request.MonthlyAmount < 0) throw new ArgumentException("Invalid budget.");
        var category = request.Category.Trim();
        var budget = await repository.GetBudgetByCategoryAsync(category, cancellationToken);
        if (budget is null)
        {
            budget = new Budget { Category = category, MonthlyAmount = request.MonthlyAmount };
            repository.AddBudget(budget);
        }
        else
        {
            budget.MonthlyAmount = request.MonthlyAmount;
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return mapper.Map<BudgetResponse>(budget);
    }

    public async Task DeleteBudgetAsync(int id, CancellationToken cancellationToken = default)
    {
        var budget = await repository.GetBudgetByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException("Budget not found.");
        repository.RemoveBudget(budget);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<PagedResult<ExpenseResponse>> GetExpensesAsync(int page, int pageSize, string? category, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var data = await repository.GetExpensesAsync(page, pageSize, category, cancellationToken);
        var total = await repository.CountExpensesAsync(category, cancellationToken);
        return new PagedResult<ExpenseResponse>(mapper.Map<IReadOnlyList<ExpenseResponse>>(data), page, pageSize, total);
    }

    public async Task<ExpenseResponse> CreateExpenseAsync(CreateExpenseRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Category) || request.Amount <= 0) throw new ArgumentException("Category and a positive amount are required.");
        var expense = new Expense
        {
            Category = request.Category.Trim(),
            Amount = request.Amount,
            Date = request.Date == default ? DateTime.UtcNow : request.Date,
            Description = request.Description?.Trim() ?? string.Empty
        };
        repository.AddExpense(expense);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return mapper.Map<ExpenseResponse>(expense);
    }

    public async Task DeleteExpenseAsync(int id, CancellationToken cancellationToken = default)
    {
        var expense = await repository.GetExpenseAsync(id, cancellationToken) ?? throw new KeyNotFoundException("Expense not found.");
        repository.RemoveExpense(expense);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<InvoiceResponse>> GetInvoicesAsync(InvoiceStatus? status, CancellationToken cancellationToken = default)
        => mapper.Map<IReadOnlyList<InvoiceResponse>>(await repository.GetInvoicesAsync(status, cancellationToken));

    public async Task<InvoiceResponse> GetInvoiceAsync(int id, CancellationToken cancellationToken = default)
        => mapper.Map<InvoiceResponse>(await repository.GetInvoiceAsync(id, cancellationToken) ?? throw new KeyNotFoundException("Invoice not found."));

    public async Task<InvoiceResponse> CreateInvoiceFromOrderAsync(int orderId, CancellationToken cancellationToken = default)
    {
        var order = await repository.GetDeliveredOrderAsync(orderId, cancellationToken) ?? throw new KeyNotFoundException("Order not found.");
        if (order.Status != OrderStatus.Delivered) throw new InvalidOperationException("Invoice can only be created for delivered orders.");
        if (await repository.InvoiceExistsForOrderAsync(orderId, cancellationToken)) throw new InvalidOperationException("Invoice already exists.");
        var days = order.Customer.PaymentTerms switch { "Due on Receipt" => 0, "Net 15" => 15, "Net 45" => 45, "Net 60" => 60, _ => 30 };
        var issued = DateTime.UtcNow;
        var invoice = new Invoice { OrderId = order.Id, CustomerId = order.CustomerId, CustomerName = order.Customer.Company, Amount = order.Total, Issued = issued, Due = issued.AddDays(days) };
        repository.AddInvoice(invoice);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return mapper.Map<InvoiceResponse>(invoice);
    }

    public async Task<InvoiceResponse> PayInvoiceAsync(int id, CancellationToken cancellationToken = default)
    {
        var invoice = await repository.GetInvoiceAsync(id, cancellationToken) ?? throw new KeyNotFoundException("Invoice not found.");
        if (invoice.Status != InvoiceStatus.Paid)
        {
            invoice.Status = InvoiceStatus.Paid;
            invoice.PaidOn = DateTime.UtcNow;
            if (invoice.OrderId is int orderId)
            {
                var paid = await repository.GetOrderPaymentsAsync(orderId, cancellationToken);
                if (paid < invoice.Amount)
                    repository.AddPayment(new Payment { OrderId = orderId, Amount = invoice.Amount - paid, Method = PaymentMethod.Other });
            }
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        return mapper.Map<InvoiceResponse>(invoice);
    }
}
