using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Application.Features.Auth.Contracts;

public sealed record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt,
    int UserId,
    string DisplayName,
    string Email,
    Role Role);
