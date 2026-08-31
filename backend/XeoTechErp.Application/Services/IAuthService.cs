using XeoTechErp.Application.Common;
using XeoTechErp.Application.Contracts.Auth;

namespace XeoTechErp.Application.Services;

public interface IAuthService
{
    Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
