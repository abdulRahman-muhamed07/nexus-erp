using System.ComponentModel.DataAnnotations;

namespace XeoTechErp.Application.Features.Auth.Contracts;

public sealed record RegisterRequest(
    [property: Required, StringLength(100, MinimumLength = 2)] string DisplayName,
    [property: Required, EmailAddress, StringLength(200)] string Email,
    [property: Required, StringLength(100, MinimumLength = 8)] string Password);
