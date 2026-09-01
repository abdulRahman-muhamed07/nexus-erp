using XeoTechErp.Application.Contracts.Reports;

namespace XeoTechErp.Application.Services;

public interface IReportService
{
    Task<SalesSummaryResponse> GetSalesSummaryAsync(CancellationToken cancellationToken = default);
}
