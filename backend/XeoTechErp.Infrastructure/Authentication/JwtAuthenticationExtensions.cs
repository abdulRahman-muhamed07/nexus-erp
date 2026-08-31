using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using XeoTechErp.Infrastructure.Authentication;

namespace XeoTechErp.Infrastructure.Authentication;

public static class JwtAuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        bool isDevelopment)
    {
        var section = configuration.GetSection(JwtOptions.SectionName);
        services
            .AddOptions<JwtOptions>()
            .Bind(section)
            .Validate(options => !string.IsNullOrWhiteSpace(options.Key), "Jwt:Key is required.")
            .Validate(options => options.Key.Length >= 32, "Jwt:Key must be at least 32 characters.")
            .Validate(options => !options.Key.StartsWith("CHANGE_ME", StringComparison.OrdinalIgnoreCase), "Jwt:Key must not use a placeholder value.")
            .Validate(options => options.AccessTokenLifetimeMinutes is > 0 and <= 60, "Jwt:AccessTokenLifetimeMinutes must be between 1 and 60 minutes.")
            .Validate(options => options.RefreshTokenLifetimeDays is >= 1 and <= 30, "Jwt:RefreshTokenLifetimeDays must be between 1 and 30 days.")
            .ValidateOnStart();

        var jwt = section.Get<JwtOptions>() ?? new JwtOptions();
        if (string.IsNullOrWhiteSpace(jwt.Key) || jwt.Key.StartsWith("CHANGE_ME", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("A real Jwt:Key must be provided through secure configuration.");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = !isDevelopment;
                options.SaveToken = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Administrator"));
            options.AddPolicy("ManagerOrAdmin", policy => policy.RequireRole("Manager", "Administrator"));
        });

        return services;
    }
}
