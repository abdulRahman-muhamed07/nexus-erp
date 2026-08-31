namespace XeoTechErp.Application.Features.Finance.Dashboard;

public sealed record PeriodFinanceSummaryDto(DateTime From, DateTime To, decimal Revenue, decimal Returns, decimal Expenses, decimal Net);
