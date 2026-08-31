namespace XeoTechErp.Application.Abstractions.Authentication;

public interface IAuthenticationSettings
{
    int RefreshTokenLifetimeDays { get; }
}
