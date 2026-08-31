using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using XeoTechErp.Application.Common;
using XeoTechErp.Application.Contracts.Orders;
using XeoTechErp.Application.CQRS;
using XeoTechErp.Application.Features.Finance;
using XeoTechErp.Application.Features.Orders.Commands.CreateOrder;
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
        services.AddAutoMapper(typeof(ApplicationMappingProfile).Assembly);
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IFinanceService, FinanceService>();
        services.AddScoped<IQuoteService, QuoteService>();
        services.AddScoped<IActivityService, ActivityService>();
        services.AddScoped<IPaymentService, PaymentService>();

        services.AddScoped<ICommandHandler<CreateOrderCommand, Result<OrderDto>>, CreateOrderCommandHandler>();
        services.AddScoped<IQueryHandler<GetOrderQuery, OrderDto?>, GetOrderQueryHandler>();
        services.AddScoped<IQueryHandler<GetOrdersQuery, IReadOnlyList<OrderDto>>, GetOrdersQueryHandler>();

        return services;
    }
}
