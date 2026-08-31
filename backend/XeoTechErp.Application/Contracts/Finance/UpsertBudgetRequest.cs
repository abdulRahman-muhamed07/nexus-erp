namespace XeoTechErp.Application.Contracts.Finance;

public sealed record UpsertBudgetRequest(string Category, decimal MonthlyAmount);
