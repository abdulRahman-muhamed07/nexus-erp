using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Application.Contracts.Auth;

public sealed record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt,
    int UserId,
    string DisplayName,
    string Email,
    Role Role);
