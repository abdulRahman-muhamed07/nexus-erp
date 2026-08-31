namespace XeoTechErp.Application.Features.Finance.Budgets;

public sealed record UpsertBudgetRequest(string Category, decimal MonthlyAmount);
