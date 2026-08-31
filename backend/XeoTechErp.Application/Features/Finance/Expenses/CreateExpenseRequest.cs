namespace XeoTechErp.Application.Features.Finance.Expenses;

public sealed record CreateExpenseRequest(string Category, decimal Amount, DateTime Date, string Description);
