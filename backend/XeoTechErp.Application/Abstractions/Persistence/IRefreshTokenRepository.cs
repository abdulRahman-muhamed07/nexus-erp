using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Application.Abstractions.Persistence;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetActiveAsync(string tokenHash, CancellationToken cancellationToken = default);
    void Add(RefreshToken refreshToken);
    Task RevokeAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task RevokeAllForUserAsync(int userId, CancellationToken cancellationToken = default);
}
