using System.Security.Cryptography;
using System.Text;
using XeoTechErp.Application.Abstractions.Authentication;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Application.Abstractions.Services;
using XeoTechErp.Application.Common;
using XeoTechErp.Application.Contracts.Auth;
using XeoTechErp.Domain.Entities;
using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Application.Services;

public sealed class AuthService(
    IUserRepository users,
    IRefreshTokenRepository refreshTokens,
    IPasswordVerifier passwordVerifier,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IAuthenticationSettings authenticationSettings,
    IUnitOfWork unitOfWork) : IAuthService
{
    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var email = NormalizeEmail(request.Email);
        var displayName = request.DisplayName?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(displayName) || displayName.Length < 2)
            return Result<AuthResponse>.Failure("INVALID_NAME", "Display name is required.");

        if (!IsStrongPassword(request.Password))
            return Result<AuthResponse>.Failure("WEAK_PASSWORD", "Password must be at least 8 characters and contain upper, lower, digit and special characters.");

        if (await users.GetByEmailAsync(email, cancellationToken) is not null)
            return Result<AuthResponse>.Failure("EMAIL_EXISTS", "An account with this email already exists.");

        var user = new User
        {
            Email = email,
            DisplayName = displayName,
            PasswordHash = passwordHasher.Hash(request.Password),
            Role = Role.Viewer
        };

        users.Add(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await IssueTokensAsync(user, cancellationToken);
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var email = NormalizeEmail(request.Email);
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(request.Password))
            return Result<AuthResponse>.Failure("INVALID_CREDENTIALS", "Invalid email or password.");

        var user = await users.GetByEmailAsync(email, cancellationToken);
        if (user is null || !passwordVerifier.Verify(user, request.Password))
            return Result<AuthResponse>.Failure("INVALID_CREDENTIALS", "Invalid email or password.");

        return await IssueTokensAsync(user, cancellationToken);
    }

    public async Task<Result<AuthResponse>> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return Result<AuthResponse>.Failure("INVALID_REFRESH_TOKEN", "Invalid refresh token.");

        var stored = await refreshTokens.GetActiveAsync(HashToken(request.RefreshToken), cancellationToken);
        if (stored?.User is null)
            return Result<AuthResponse>.Failure("INVALID_REFRESH_TOKEN", "Invalid or expired refresh token.");

        stored.RevokedAt = DateTime.UtcNow;
        return await IssueTokensAsync(stored.User, cancellationToken);
    }

    public async Task<Result> LogoutAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return Result.Failure("INVALID_REFRESH_TOKEN", "Refresh token is required.");

        var stored = await refreshTokens.GetActiveAsync(HashToken(refreshToken), cancellationToken);
        if (stored is null)
            return Result.Success();

        stored.RevokedAt = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> UpdateRoleAsync(int userId, UpdateRoleRequest request, CancellationToken cancellationToken = default)
    {
        if (!Enum.IsDefined(request.Role))
            return Result.Failure("INVALID_ROLE", "Unsupported role.");

        var user = await users.GetByIdAsync(userId, cancellationToken);
        if (user is null)
            return Result.Failure("USER_NOT_FOUND", "User not found.");

        if (user.Role == request.Role)
            return Result.Success();

        user.Role = request.Role;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private async Task<Result<AuthResponse>> IssueTokensAsync(User user, CancellationToken cancellationToken)
    {
        var accessToken = tokenService.CreateAccessToken(user);
        var rawRefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = HashToken(rawRefreshToken),
            ExpiresAt = DateTime.UtcNow.AddDays(authenticationSettings.RefreshTokenLifetimeDays)
        };

        refreshTokens.Add(refreshToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<AuthResponse>.Success(new AuthResponse(
            accessToken.AccessToken,
            rawRefreshToken,
            accessToken.ExpiresAt,
            refreshToken.ExpiresAt,
            user.Id,
            user.DisplayName,
            user.Email,
            user.Role));
    }

    private static string NormalizeEmail(string? email) => email?.Trim().ToLowerInvariant() ?? string.Empty;

    private static string HashToken(string token)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

    private static bool IsStrongPassword(string? password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            return false;

        return password.Any(char.IsUpper)
               && password.Any(char.IsLower)
               && password.Any(char.IsDigit)
               && password.Any(ch => !char.IsLetterOrDigit(ch));
    }
}
