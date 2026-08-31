using Microsoft.EntityFrameworkCore;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Infrastructure.Persistence.Repositories;

public sealed class RefreshTokenRepository(XeoTechDbContext db) : IRefreshTokenRepository
{
    public Task<RefreshToken?> GetActiveAsync(string tokenHash, CancellationToken cancellationToken = default)
        => db.RefreshTokens
            .Include(x => x.User)
            .SingleOrDefaultAsync(
                x => x.TokenHash == tokenHash && x.RevokedAt == null && x.ExpiresAt > DateTime.UtcNow,
                cancellationToken);

    public void Add(RefreshToken refreshToken) => db.RefreshTokens.Add(refreshToken);

    public Task RevokeAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        refreshToken.RevokedAt = DateTime.UtcNow;
        return Task.CompletedTask;
    }

    public async Task RevokeAllForUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        var tokens = await db.RefreshTokens
            .Where(x => x.UserId == userId && x.RevokedAt == null)
            .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;
        foreach (var token in tokens)
            token.RevokedAt = now;
    }
}
