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

        try
        {
            var order = new Order(customer.Id);
            foreach (var item in request.Items)
            {
                if (item.Qty <= 0)
                    return Result<OrderDto>.Failure("INVALID_QUANTITY", "Quantity must be greater than zero.");
                order.AddItem(productsById[item.ProductId], item.Qty);
            }

            order.ApplyDiscount(request.DiscountPct);

            var config = await appConfig.GetAsync(cancellationToken) ?? new AppConfig();
            var taxable = order.Subtotal - order.Discount;
            var tax = Math.Round(taxable * config.TaxRate / 100m, 2);
            var shipping = taxable >= config.FreeShipOver ? 0m : config.ShippingFee;
            order.SetCharges(tax, shipping);

            await unitOfWork.ExecuteInTransactionAsync(async () =>
            {
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
            }, cancellationToken);

            return Result<OrderDto>.Success(mapper.Map<OrderDto>(order));
        }
        catch (DomainRuleException ex)
        {
            return Result<OrderDto>.Failure("ORDER_RULE_VIOLATION", ex.Message);
        }
    }
}
