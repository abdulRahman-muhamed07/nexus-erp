namespace XeoTechErp.Application.Features.Finance;

public sealed record FinanceSummaryDto(decimal Revenue, decimal Collections, decimal Receivables, decimal Refunds, decimal NetRevenue, decimal Profit);
public sealed record AgingBucketDto(string Bucket, decimal Total, int Count);
public sealed record BudgetVarianceDto(string Category, decimal Budget, decimal Actual, decimal Variance);
public sealed record PeriodFinanceSummaryDto(DateTime From, DateTime To, decimal Revenue, decimal Returns, decimal Expenses, decimal Net);
