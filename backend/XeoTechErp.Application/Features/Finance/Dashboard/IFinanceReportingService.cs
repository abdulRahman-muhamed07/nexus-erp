using XeoTechErp.Application.Common.Models;

namespace XeoTechErp.Application.Features.Finance.Dashboard;

public interface IFinanceReportingService
{
    Task<FinanceSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AgingBucketDto>> GetAgingAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BudgetVarianceDto>> GetBudgetVarianceAsync(CancellationToken cancellationToken = default);
    Task<PeriodFinanceSummaryDto> GetPeriodSummaryAsync(DateTime? from, DateTime? to, CancellationToken cancellationToken = default);
}
