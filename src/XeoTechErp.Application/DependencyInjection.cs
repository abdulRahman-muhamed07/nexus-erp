using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using XeoTechErp.Application.Abstractions.Messaging;
using XeoTechErp.Application.Abstractions.Services;
using XeoTechErp.Application.Common;
using XeoTechErp.Application.Features.Activities;
using XeoTechErp.Application.Features.Auth;
using XeoTechErp.Application.Features.Finance.Assets;
using XeoTechErp.Application.Features.Finance.Budgets;
using XeoTechErp.Application.Features.Finance.Dashboard;
using XeoTechErp.Application.Features.Finance.Expenses;
using XeoTechErp.Application.Features.Finance.Invoices;
using XeoTechErp.Application.Features.Orders.Commands.CreateOrder;
using XeoTechErp.Application.Features.Orders.Common;
using XeoTechErp.Application.Features.Orders.Queries.GetOrder;
using XeoTechErp.Application.Features.Orders.Queries.GetOrders;
using XeoTechErp.Application.Features.Quotes;
using XeoTechErp.Application.Mapping;
using XeoTechErp.Application.Services;

namespace XeoTechErp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddProfile<ApplicationMappingProfile>());

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
        services.AddScoped<IQuoteService, QuoteService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IReturnService, ReturnService>();
        services.AddScoped<ISettingsService, SettingsService>();
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<IActivityService, ActivityService>();
        services.AddScoped<IAuditService, AuditService>();

        services.AddScoped<IFinanceReportingService, FinanceReportingService>();
        services.AddScoped<IAssetService, AssetService>();
        services.AddScoped<IBudgetService, BudgetService>();
        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddScoped<IInvoiceService, InvoiceService>();

        services.AddScoped<ICommandHandler<CreateOrderCommand, Result<OrderDto>>, CreateOrderCommandHandler>();
        services.AddScoped<IQueryHandler<GetOrderQuery, OrderDto?>, GetOrderQueryHandler>();
        services.AddScoped<IQueryHandler<GetOrdersQuery, IReadOnlyList<OrderDto>>, GetOrdersQueryHandler>();

        return services;
    }
}
