using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Application.Contracts.Auth;

public sealed record LoginRequest(string Email, string Password);
public sealed record LoginResponse(string Token, DateTime ExpiresAt, int UserId, string DisplayName, Role Role);
