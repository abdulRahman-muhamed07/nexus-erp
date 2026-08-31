namespace XeoTechErp.Api.DTOs;

public sealed record PagedResponse<T>(IReadOnlyCollection<T> Data, int Page, int PageSize, int Total);
public sealed record ApiError(string Code, string Message, string? TraceId = null);
public sealed record RefreshTokenRequest(string RefreshToken);
public sealed record ExpenseRequest(string Category, decimal Amount, DateTime Date, string? Description);
public sealed record CreateBudgetRequest(string Category, decimal MonthlyAmount);
