namespace XeoTechErp.Application.Features.Finance.Dashboard;

public sealed record FinanceSummaryDto(decimal Revenue, decimal Collections, decimal Receivables, decimal Refunds, decimal NetRevenue, decimal Profit);
