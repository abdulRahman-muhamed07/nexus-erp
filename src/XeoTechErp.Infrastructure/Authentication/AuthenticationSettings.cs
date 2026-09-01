using Microsoft.Extensions.Options;
using XeoTechErp.Application.Abstractions.Authentication;

namespace XeoTechErp.Infrastructure.Authentication;

public sealed class AuthenticationSettings(IOptions<JwtOptions> options) : IAuthenticationSettings
{
    public int RefreshTokenLifetimeDays => options.Value.RefreshTokenLifetimeDays;
}
