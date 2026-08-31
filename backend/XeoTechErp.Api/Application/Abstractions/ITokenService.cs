using XeoTechErp.Api.Domain.Entities;

namespace XeoTechErp.Api.Application.Abstractions;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) CreateAccessToken(User user);
}