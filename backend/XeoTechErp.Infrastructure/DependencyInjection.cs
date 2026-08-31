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
        var connectionString = configuration.GetConnectionString("Default") ?? "Data Source=xeotech-erp.db";
        services.AddDbContext<XeoTechDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IFinanceRepository, FinanceRepository>();
        services.AddScoped<IPasswordVerifier, PasswordVerifier>();
        services.AddSingleton<ITokenService>(_ => new JwtTokenService(configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is required.")));
        return services;
    }
}
