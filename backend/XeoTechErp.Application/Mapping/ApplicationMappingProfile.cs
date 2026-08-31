using AutoMapper;
using XeoTechErp.Application.Contracts.Activities;
using XeoTechErp.Application.Contracts.Audit;
using XeoTechErp.Application.Contracts.Customers;
using XeoTechErp.Application.Contracts.Employees;
using XeoTechErp.Application.Contracts.Notifications;
using XeoTechErp.Application.Contracts.Payments;
using XeoTechErp.Application.Contracts.Products;
using XeoTechErp.Application.Contracts.PurchaseOrders;
using XeoTechErp.Application.Contracts.Returns;
using XeoTechErp.Application.Contracts.Settings;
using XeoTechErp.Application.Contracts.Suppliers;
using XeoTechErp.Application.Features.Finance.Assets;
using XeoTechErp.Application.Features.Finance.Budgets;
using XeoTechErp.Application.Features.Finance.Expenses;
using XeoTechErp.Application.Features.Finance.Invoices;
using XeoTechErp.Application.Features.Orders.Common;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Application.Mapping;

public sealed class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<Product, ProductDto>();
        CreateMap<Customer, CustomerDto>();
        CreateMap<Order, OrderDto>();
        CreateMap<Activity, ActivityResponse>();
        CreateMap<Employee, EmployeeResponse>();
        CreateMap<Notification, NotificationResponse>();
        CreateMap<Payment, PaymentResponse>();
        CreateMap<AuditLogEntry, AuditLogResponse>();
        CreateMap<Supplier, SupplierResponse>();
        CreateMap<PurchaseOrder, PurchaseOrderResponse>();
        CreateMap<Return, ReturnResponse>();
        CreateMap<AppConfig, AppConfigResponse>();
        CreateMap<Asset, AssetResponse>();
        CreateMap<Budget, BudgetResponse>();
        CreateMap<Expense, ExpenseResponse>();
        CreateMap<Invoice, InvoiceResponse>();
    }
}
