using AutoMapper;
using XeoTechErp.Application.Contracts.Auth;
using XeoTechErp.Application.Contracts.Customers;
using XeoTechErp.Application.Contracts.Orders;
using XeoTechErp.Application.Contracts.Products;
using XeoTechErp.Application.Features.Finance;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Application.Mapping;

public sealed class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<Product, ProductDto>();
        CreateMap<Customer, CustomerDto>();
        CreateMap<Order, OrderDto>();
        CreateMap<FinanceSummaryDto, FinanceSummaryDto>();

        CreateMap<LoginRequest, LoginRequest>();
        CreateMap<Activity, ActivityResponse>();
        CreateMap<Employee, EmployeeResponse>();
        CreateMap<Notification, NotificationResponse>();
        CreateMap<Payment, PaymentResponse>();
        CreateMap<AuditLogEntry, AuditLogResponse>();
    }
}
