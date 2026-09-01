using Microsoft.EntityFrameworkCore;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Application.Features.Finance.Dashboard;
using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Infrastructure.Persistence.Repositories;

public sealed class FinanceReportingRepository(XeoTechDbContext db) : IFinanceReportingRepository
{
    public async Task<FinanceSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var revenue = await db.Orders
            .Where(x => x.Status != OrderStatus.Cancelled)
            .SumAsync(x => (decimal?)x.Total, cancellationToken) ?? 0m;
        var receivables = await db.Invoices
            .Where(x => x.Status != InvoiceStatus.Paid)
            .SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0m;
        var collections = await db.Payments.SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0m;
        var refunds = await db.Returns.SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0m;
        return new FinanceSummaryDto(revenue, collections, receivables, refunds, revenue - refunds, revenue - refunds - collections);
    }

    public async Task<IReadOnlyList<AgingBucketDto>> GetAgingAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var rows = await db.Invoices.AsNoTracking()
            .Where(x => x.Status != InvoiceStatus.Paid)
            .Select(x => new { x.Amount, x.Due })
            .ToListAsync(cancellationToken);

        return rows
            .GroupBy(x => x.Due >= now
                ? "Current"
                : (now - x.Due).TotalDays <= 30 ? "1-30"
                : (now - x.Due).TotalDays <= 60 ? "31-60"
                : (now - x.Due).TotalDays <= 90 ? "61-90"
                : "90+")
            .Select(g => new AgingBucketDto(g.Key, g.Sum(x => x.Amount), g.Count()))
            .ToList();
    }

    public async Task<IReadOnlyList<BudgetVarianceDto>> GetBudgetVarianceAsync(CancellationToken cancellationToken = default)
    {
        var budgets = await db.Budgets.AsNoTracking().OrderBy(x => x.Category).ToListAsync(cancellationToken);
        return budgets.Select(x => new BudgetVarianceDto(x.Category, x.MonthlyAmount, 0m, x.MonthlyAmount)).ToList();
    }

    public async Task<PeriodFinanceSummaryDto> GetPeriodSummaryAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var revenue = await db.Payments
            .Where(x => x.Date >= from && x.Date <= to)
            .SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0m;
        var returns = await db.Returns
            .Where(x => x.Date >= from && x.Date <= to)
            .SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0m;
        var expenses = await db.Expenses
            .Where(x => x.Date >= from && x.Date <= to)
            .SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0m;
        return new PeriodFinanceSummaryDto(from, to, revenue, returns, expenses, revenue - returns - expenses);
    }
}