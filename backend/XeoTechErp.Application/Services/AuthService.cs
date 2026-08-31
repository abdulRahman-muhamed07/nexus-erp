using XeoTechErp.Application.Abstractions.Authentication;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Application.Common;
using XeoTechErp.Application.Contracts.Auth;

namespace XeoTechErp.Application.Services;

public interface IAuthService
{
    Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}

public sealed class AuthService(IUserRepository users, IPasswordVerifier passwordVerifier, ITokenService tokenService) : IAuthService
{
    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return Result<LoginResponse>.Failure("INVALID_CREDENTIALS", "Invalid email or password.");
        var user = await users.GetByEmailAsync(request.Email.Trim().ToLowerInvariant(), cancellationToken);
        if (user is null || !passwordVerifier.Verify(user, request.Password))
            return Result<LoginResponse>.Failure("INVALID_CREDENTIALS", "Invalid email or password.");
        var token = tokenService.CreateAccessToken(user);
        return Result<LoginResponse>.Success(new LoginResponse(token.AccessToken, token.ExpiresAt, user.Id, user.DisplayName, user.Role));
    }
}
