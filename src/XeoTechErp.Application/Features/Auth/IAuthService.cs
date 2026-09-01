using XeoTechErp.Application.Common;
using XeoTechErp.Application.Contracts.Auth;

namespace XeoTechErp.Application.Features.Auth;

public interface IAuthService
{
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
    Task<Result> LogoutAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<Result> UpdateRoleAsync(int userId, UpdateRoleRequest request, CancellationToken cancellationToken = default);
}
