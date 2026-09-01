using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Application.Contracts.Auth;

public sealed record LoginResponse(string Token, DateTime ExpiresAt, int UserId, string DisplayName, Role Role);
