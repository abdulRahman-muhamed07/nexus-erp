using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Application.Common;
using XeoTechErp.Application.Contracts.Orders;
using XeoTechErp.Application.CQRS;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Application.Features.Orders.Commands.CreateOrder;

public sealed class CreateOrderCommandHandler(
    IOrderRepository orders,
    ICustomerRepository customers,
    IProductRepository products,
    IInventoryRepository inventory,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateOrderCommand, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken = default)
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
            if (item.Qty <= 0)
                return Result<OrderDto>.Failure("INVALID_QUANTITY", "Quantity must be greater than zero.");

            try
            {
                order.AddItem(productsById[item.ProductId], item.Qty);
            }
            catch (XeoTechErp.Domain.Exceptions.DomainRuleException ex)
            {
                return Result<OrderDto>.Failure("ORDER_RULE_VIOLATION", ex.Message);
            }
        }

        try
        {
            order.ApplyDiscount(request.DiscountPct);
        }
        catch (XeoTechErp.Domain.Exceptions.DomainRuleException ex)
        {
            return Result<OrderDto>.Failure("INVALID_DISCOUNT", ex.Message);
        }

        var taxable = order.Subtotal - order.Discount;
        var tax = Math.Round(taxable * 0.08m, 2);
        var shipping = taxable >= 1000m ? 0m : 25m;
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

        return Result<OrderDto>.Success(new OrderDto(
            order.Id,
            order.CustomerId,
            order.Status,
            order.OrderDate,
            order.Subtotal,
            order.Tax,
            order.Shipping,
            order.Discount,
            order.Total));
    }
}
