using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XeoTechErp.Api.Application.Abstractions;
using XeoTechErp.Api.Data;
using XeoTechErp.Api.Infrastructure.Persistence;
using XeoTechErp.Api.Infrastructure.Persistence.Repositories;
using XeoTechErp.Api.Infrastructure.Security;

namespace XeoTechErp.Api.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<XeoTechDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("Default") ?? "Data Source=xeotech-erp.db"));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();
        services.AddScoped<ITokenService, JwtTokenService>();

        return services;
    }
}