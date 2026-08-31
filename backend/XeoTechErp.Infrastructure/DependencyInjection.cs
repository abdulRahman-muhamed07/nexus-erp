using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XeoTechErp.Application.Abstractions.Authentication;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Infrastructure.Authentication;
using XeoTechErp.Infrastructure.Persistence;
using XeoTechErp.Infrastructure.Persistence.Repositories;

namespace XeoTechErp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("ConnectionStrings:Default is required.");

        var jwtKey = configuration["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.StartsWith("CHANGE_ME", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Jwt:Key must be provided through secure configuration.");

        var jwtIssuer = configuration["Jwt:Issuer"] ?? "XeoTechErp.Api";
        var jwtAudience = configuration["Jwt:Audience"] ?? "XeoTechErp.Client";

        services.AddDbContext<XeoTechDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IFinanceRepository, FinanceRepository>();
        services.AddScoped<IQuoteRepository, QuoteRepository>();
        services.AddScoped<IActivityRepository, ActivityRepository>();
        services.AddScoped<IPasswordVerifier, PasswordVerifier>();
        services.AddSingleton<ITokenService>(_ => new JwtTokenService(jwtKey, jwtIssuer, jwtAudience));

        return services;
    }
}
