namespace XeoTechErp.Application.Features.Finance.Expenses;

public sealed record ExpenseResponse(int Id, string Category, decimal Amount, DateTime Date, string Description);
