namespace XeoTechErp.Infrastructure.Authentication;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Key { get; init; } = string.Empty;
    public string Issuer { get; init; } = "XeoTechErp.Api";
    public string Audience { get; init; } = "XeoTechErp.Client";
    public int AccessTokenLifetimeMinutes { get; init; } = 480;
}
