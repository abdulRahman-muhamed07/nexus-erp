using Microsoft.EntityFrameworkCore;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Application.Features.Finance;
using XeoTechErp.Domain.Entities;
using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Infrastructure.Persistence.Repositories;

public sealed class FinanceRepository(XeoTechDbContext db) : IFinanceRepository
{
    public async Task<FinanceSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var revenue = await db.Orders.Where(x => x.Status != OrderStatus.Cancelled).SumAsync(x => (decimal?)x.Total, cancellationToken) ?? 0m;
        var receivables = await db.Invoices.Where(x => x.Status != InvoiceStatus.Paid).SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0m;
        var collections = await db.Payments.SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0m;
        var refunds = await db.Returns.SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0m;
        return new FinanceSummaryDto(revenue, collections, receivables, refunds, revenue - refunds, revenue - refunds - collections);
    }

    public async Task<IReadOnlyList<AgingBucketDto>> GetAgingAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var rows = await db.Invoices.AsNoTracking().Where(x => x.Status != InvoiceStatus.Paid)
            .Select(x => new { x.Amount, x.Due }).ToListAsync(cancellationToken);
        return rows.GroupBy(x => x.Due >= now ? "Current" : (now - x.Due).TotalDays <= 30 ? "1-30" : (now - x.Due).TotalDays <= 60 ? "31-60" : (now - x.Due).TotalDays <= 90 ? "61-90" : "90+")
            .Select(g => new AgingBucketDto(g.Key, g.Sum(x => x.Amount), g.Count())).ToList();
    }

    public async Task<IReadOnlyList<BudgetVarianceDto>> GetBudgetVarianceAsync(CancellationToken cancellationToken = default)
    {
        var budgets = await db.Budgets.AsNoTracking().OrderBy(x => x.Category).ToListAsync(cancellationToken);
        return budgets.Select(x => new BudgetVarianceDto(x.Category, x.MonthlyAmount, 0m, x.MonthlyAmount)).ToList();
    }

    public async Task<PeriodFinanceSummaryDto> GetPeriodSummaryAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var revenue = await db.Payments.Where(x => x.Date >= from && x.Date <= to).SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0m;
        var returns = await db.Returns.Where(x => x.Date >= from && x.Date <= to).SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0m;
        var expenses = await db.Expenses.Where(x => x.Date >= from && x.Date <= to).SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0m;
        return new PeriodFinanceSummaryDto(from, to, revenue, returns, expenses, revenue - returns - expenses);
    }

    public async Task<IReadOnlyList<Asset>> GetAssetsAsync(CancellationToken cancellationToken = default) => await db.Assets.AsNoTracking().OrderByDescending(x => x.PurchaseDate).ToListAsync(cancellationToken);
    public Task<Asset?> GetAssetAsync(int id, CancellationToken cancellationToken = default) => db.Assets.FindAsync([id], cancellationToken).AsTask();
    public void AddAsset(Asset asset) => db.Assets.Add(asset);
    public async Task<IReadOnlyList<Budget>> GetBudgetsAsync(CancellationToken cancellationToken = default) => await db.Budgets.AsNoTracking().OrderBy(x => x.Category).ToListAsync(cancellationToken);
    public Task<Budget?> GetBudgetByIdAsync(int id, CancellationToken cancellationToken = default) => db.Budgets.FindAsync([id], cancellationToken).AsTask();
    public Task<Budget?> GetBudgetByCategoryAsync(string category, CancellationToken cancellationToken = default) => db.Budgets.FirstOrDefaultAsync(x => x.Category == category, cancellationToken);
    public void AddBudget(Budget budget) => db.Budgets.Add(budget);
    public void RemoveBudget(Budget budget) => db.Budgets.Remove(budget);
    public async Task<IReadOnlyList<Expense>> GetExpensesAsync(int page, int pageSize, string? category, CancellationToken cancellationToken = default)
    { var query = db.Expenses.AsNoTracking(); if (!string.IsNullOrWhiteSpace(category)) query = query.Where(x => x.Category == category); return await query.OrderByDescending(x => x.Date).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken); }
    public async Task<int> CountExpensesAsync(string? category, CancellationToken cancellationToken = default)
    { var query = db.Expenses.AsNoTracking(); if (!string.IsNullOrWhiteSpace(category)) query = query.Where(x => x.Category == category); return await query.CountAsync(cancellationToken); }
    public Task<Expense?> GetExpenseAsync(int id, CancellationToken cancellationToken = default) => db.Expenses.FindAsync([id], cancellationToken).AsTask();
    public void AddExpense(Expense expense) => db.Expenses.Add(expense);
    public void RemoveExpense(Expense expense) => db.Expenses.Remove(expense);
    public async Task<IReadOnlyList<Invoice>> GetInvoicesAsync(InvoiceStatus? status, CancellationToken cancellationToken = default)
    { var query = db.Invoices.AsNoTracking().Include(x => x.Customer).Include(x => x.Order); if (status.HasValue) query = query.Where(x => x.Status == status); return await query.OrderByDescending(x => x.Issued).ToListAsync(cancellationToken); }
    public Task<Invoice?> GetInvoiceAsync(int id, CancellationToken cancellationToken = default) => db.Invoices.Include(x => x.Customer).Include(x => x.Order).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    public Task<Order?> GetDeliveredOrderAsync(int orderId, CancellationToken cancellationToken = default) => db.Orders.Include(x => x.Customer).FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken);
    public Task<bool> InvoiceExistsForOrderAsync(int orderId, CancellationToken cancellationToken = default) => db.Invoices.AnyAsync(x => x.OrderId == orderId, cancellationToken);
    public async Task<decimal> GetOrderPaymentsAsync(int orderId, CancellationToken cancellationToken = default) => await db.Payments.Where(x => x.OrderId == orderId).SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0m;
    public void AddInvoice(Invoice invoice) => db.Invoices.Add(invoice);
    public void AddPayment(Payment payment) => db.Payments.Add(payment);
}
