namespace XeoTechErp.Application.Contracts.Reports;

public sealed record SalesSummaryResponse(int TotalOrders, decimal Revenue, decimal AverageOrderValue);
