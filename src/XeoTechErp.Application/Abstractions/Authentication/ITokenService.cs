using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Application.Abstractions.Authentication;

public interface ITokenService
{
    TokenResult CreateAccessToken(User user);
}
