namespace XeoTechErp.Application.Contracts.Finance;

public sealed record CreateExpenseRequest(string Category, decimal Amount, DateTime Date, string Description);
