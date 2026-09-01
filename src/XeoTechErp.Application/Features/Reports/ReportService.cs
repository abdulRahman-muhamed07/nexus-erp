using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Application.Contracts.Reports;

namespace XeoTechErp.Application.Services;

public sealed class ReportService(IReportRepository repository) : IReportService
{
    public async Task<SalesSummaryResponse> GetSalesSummaryAsync(CancellationToken cancellationToken = default)
    {
        var result = await repository.GetSalesSummaryAsync(cancellationToken);
        return new SalesSummaryResponse(result.TotalOrders, result.Revenue, result.AverageOrderValue);
    }
}
