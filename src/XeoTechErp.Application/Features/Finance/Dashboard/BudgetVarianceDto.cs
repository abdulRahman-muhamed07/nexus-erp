namespace XeoTechErp.Application.Features.Finance.Dashboard;

public sealed record BudgetVarianceDto(string Category, decimal Budget, decimal Actual, decimal Variance);
