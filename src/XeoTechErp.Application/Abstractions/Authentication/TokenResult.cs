namespace XeoTechErp.Application.Abstractions.Authentication;

public sealed record TokenResult(string AccessToken, DateTime ExpiresAt);
