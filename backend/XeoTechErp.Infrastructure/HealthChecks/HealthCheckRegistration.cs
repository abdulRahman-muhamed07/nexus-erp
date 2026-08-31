using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using XeoTechErp.Infrastructure.Persistence;

namespace XeoTechErp.Infrastructure.HealthChecks;

public static class HealthCheckRegistration
{
    public static IServiceCollection AddInfrastructureHealthChecks(this IServiceCollection services)
    {
        services
            .AddHealthChecks()
            .AddDbContextCheck<XeoTechDbContext>(
                name: "database",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "ready", "database" });

        return services;
    }
}
