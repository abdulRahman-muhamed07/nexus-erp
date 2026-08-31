using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using XeoTechErp.Application.Abstractions.Authentication;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Infrastructure.Authentication;

public sealed class JwtTokenService(string signingKey, string issuer, string audience) : ITokenService
{
    public TokenResult CreateAccessToken(User user)
    {
        var expires = DateTime.UtcNow.AddHours(8);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.DisplayName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        return new TokenResult(new JwtSecurityTokenHandler().WriteToken(token), expires);
    }
}
