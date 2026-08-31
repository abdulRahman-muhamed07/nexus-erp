using AutoMapper;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Application.Common;
using XeoTechErp.Application.Contracts.Orders;
using XeoTechErp.Application.CQRS;
using XeoTechErp.Domain.Entities;
using XeoTechErp.Domain.Exceptions;

namespace XeoTechErp.Application.Features.Orders.Commands.CreateOrder;

public sealed class CreateOrderCommandHandler(
    IOrderRepository orders,
    ICustomerRepository customers,
    IProductRepository products,
    IInventoryRepository inventory,
    IAppConfigRepository appConfig,
    IUnitOfWork unitOfWork,
    IMapper mapper) : ICommandHandler<CreateOrderCommand, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> HandleAsync(
        CreateOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var request = command.Request;

        if (request.Items is null || request.Items.Count == 0)
            return Result<OrderDto>.Failure("ORDER_EMPTY", "Order must contain at least one item.");

        var customer = await customers.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer is null)
            return Result<OrderDto>.Failure("CUSTOMER_NOT_FOUND", "Customer was not found.");

        if (customer.OnHold)
            return Result<OrderDto>.Failure("CUSTOMER_ON_HOLD", "Customer account is on hold.");

        var productIds = request.Items.Select(x => x.ProductId).Distinct().ToArray();
        var productsById = await products.GetByIdsAsync(productIds, cancellationToken);
        if (productsById.Count != productIds.Length)
            return Result<OrderDto>.Failure("PRODUCT_NOT_FOUND", "One or more products were not found.");

        var order = new Order(customer.Id);

        foreach (var item in request.Items)
        {
            try
            {
                order.AddItem(productsById[item.ProductId], item.Qty);
            }
            catch (DomainRuleException ex)
            {
                return Result<OrderDto>.Failure("ORDER_RULE_VIOLATION", ex.Message);
            }
        }

        try
        {
            order.ApplyDiscount(request.DiscountPct);
        }
        catch (DomainRuleException ex)
        {
            return Result<OrderDto>.Failure("INVALID_DISCOUNT", ex.Message);
        }

        var configuration = await appConfig.GetAsync(cancellationToken) ?? new AppConfig();
        var taxableAmount = order.Subtotal - order.Discount;
        var tax = Math.Round(taxableAmount * configuration.TaxRate / 100m, 2);
        var shipping = taxableAmount >= configuration.FreeShipOver
            ? 0m
            : configuration.ShippingFee;

        order.SetCharges(tax, shipping);

        foreach (var item in request.Items)
        {
            var product = productsById[item.ProductId];
            inventory.AddMovement(new StockMovement
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Delta = -item.Qty,
                Reason = "Sale",
                Reference = "Order",
                By = command.Actor
            });
        }

        orders.Add(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<OrderDto>.Success(mapper.Map<OrderDto>(order));
    }
}
