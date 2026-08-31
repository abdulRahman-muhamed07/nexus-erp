using Microsoft.AspNetCore.Identity;
using XeoTechErp.Api.Application.Abstractions;
using XeoTechErp.Api.Domain.Entities;
using XeoTechErp.Api.DTOs;

namespace XeoTechErp.Api.Application.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}

public sealed class AuthService(IUserRepository users, ITokenService tokenService) : IAuthService
{
    private readonly PasswordHasher<User> passwordHasher = new();

    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await users.GetByEmailAsync(email, cancellationToken);
        if (user is null)
            return null;

        var passwordResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (passwordResult == PasswordVerificationResult.Failed)
            return null;

        var (token, expiresAt) = tokenService.CreateAccessToken(user);
        return new LoginResponse(token, expiresAt, user.Id, user.DisplayName, user.Role);
    }
}
