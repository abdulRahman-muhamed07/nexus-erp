using Microsoft.Extensions.DependencyInjection;
using XeoTechErp.Application.Features.Finance;
using XeoTechErp.Application.Services;

namespace XeoTechErp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IFinanceService, FinanceService>();
        return services;
    }
}
