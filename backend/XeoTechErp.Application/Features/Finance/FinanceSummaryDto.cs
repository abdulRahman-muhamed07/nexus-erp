namespace XeoTechErp.Application.Features.Finance;

public sealed record FinanceSummaryDto(decimal Revenue, decimal Collections, decimal Receivables, decimal Refunds, decimal NetRevenue, decimal Profit);
