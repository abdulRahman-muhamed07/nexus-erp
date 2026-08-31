using XeoTechErp.Application.Abstractions.Persistence;

namespace XeoTechErp.Application.Features.Finance.Dashboard;

public sealed class FinanceReportingService(IFinanceRepository repository) : IFinanceReportingService
{
    public Task<FinanceSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
        => repository.GetSummaryAsync(cancellationToken);

    public Task<IReadOnlyList<AgingBucketDto>> GetAgingAsync(CancellationToken cancellationToken = default)
        => repository.GetAgingAsync(cancellationToken);

    public Task<IReadOnlyList<BudgetVarianceDto>> GetBudgetVarianceAsync(CancellationToken cancellationToken = default)
        => repository.GetBudgetVarianceAsync(cancellationToken);

    public Task<PeriodFinanceSummaryDto> GetPeriodSummaryAsync(
        DateTime? from,
        DateTime? to,
        CancellationToken cancellationToken = default)
    {
        var end = to ?? DateTime.UtcNow;
        var start = from ?? end.Date.AddDays(-30);

        if (start > end)
            throw new ArgumentException("from must be before to.");

        return repository.GetPeriodSummaryAsync(start, end, cancellationToken);
    }
}
