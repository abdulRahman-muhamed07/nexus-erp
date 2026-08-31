namespace XeoTechErp.Application.Features.Finance;

public sealed record BudgetVarianceDto(string Category, decimal Budget, decimal Actual, decimal Variance);
