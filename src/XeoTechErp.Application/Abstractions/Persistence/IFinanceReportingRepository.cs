using XeoTechErp.Application.Features.Finance.Dashboard;

namespace XeoTechErp.Application.Abstractions.Persistence;

public interface IFinanceReportingRepository
{
    Task<FinanceSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AgingBucketDto>> GetAgingAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BudgetVarianceDto>> GetBudgetVarianceAsync(CancellationToken cancellationToken = default);
    Task<PeriodFinanceSummaryDto> GetPeriodSummaryAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
}