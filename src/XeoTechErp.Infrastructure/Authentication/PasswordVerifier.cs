using Microsoft.AspNetCore.Identity;
using XeoTechErp.Application.Abstractions.Authentication;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Infrastructure.Authentication;

public sealed class PasswordVerifier : IPasswordVerifier, IPasswordHasher
{
    private readonly PasswordHasher<User> _hasher = new();

    public bool Verify(User user, string password)
        => _hasher.VerifyHashedPassword(user, user.PasswordHash, password) != PasswordVerificationResult.Failed;

    public string Hash(string password)
    {
        var user = new User();
        return _hasher.HashPassword(user, password);
    }
}
