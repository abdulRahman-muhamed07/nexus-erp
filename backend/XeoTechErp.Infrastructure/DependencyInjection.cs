using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XeoTechErp.Application.Abstractions.Authentication;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Infrastructure.Authentication;
using XeoTechErp.Infrastructure.HealthChecks;
using XeoTechErp.Infrastructure.Persistence;
using XeoTechErp.Infrastructure.Persistence.Repositories;

namespace XeoTechErp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("ConnectionStrings:Default is required.");

        services.AddDbContext<XeoTechDbContext>(options => options.UseSqlite(connectionString));
        services.AddInfrastructureHealthChecks();

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
        services.AddScoped<IAppConfigRepository, AppConfigRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IAuditRepository, AuditRepository>();
        services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
        services.AddScoped<IReturnRepository, ReturnRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IPasswordVerifier, PasswordVerifier>();
        services.AddSingleton<ITokenService, JwtTokenService>();
        return services;
    }
}
