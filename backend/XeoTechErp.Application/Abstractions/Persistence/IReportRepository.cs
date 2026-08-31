namespace XeoTechErp.Application.Abstractions.Persistence;

public interface IReportRepository
{
    Task<(int TotalOrders, decimal Revenue, decimal AverageOrderValue)> GetSalesSummaryAsync(CancellationToken cancellationToken = default);
}
