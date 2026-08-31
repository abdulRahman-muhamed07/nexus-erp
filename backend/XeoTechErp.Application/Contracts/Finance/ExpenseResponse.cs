namespace XeoTechErp.Application.Contracts.Finance;

public sealed record ExpenseResponse(int Id, string Category, decimal Amount, DateTime Date, string Description);
