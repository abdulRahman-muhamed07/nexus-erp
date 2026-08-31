using XeoTechErp.Api.Application.Abstractions;
using XeoTechErp.Api.Domain.Entities;
using XeoTechErp.Api.DTOs;

namespace XeoTechErp.Api.Application.Services;

public interface IOrderService
{
    Task<OrderDto> CreateAsync(CreateOrderRequest request, string actor, CancellationToken cancellationToken = default);
    Task<OrderDto?> GetAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default);
}

public sealed class OrderService(IOrderRepository repository, IUnitOfWork unitOfWork) : IOrderService
{
    public async Task<OrderDto> CreateAsync(CreateOrderRequest request, string actor, CancellationToken cancellationToken = default)
    {
        if (request.Items is null || request.Items.Count == 0)
            throw new ArgumentException("Order must contain at least one item.");

        var customer = await repository.GetCustomerAsync(request.CustomerId, cancellationToken)
            ?? throw new KeyNotFoundException("Customer not found.");

        if (customer.OnHold)
            throw new InvalidOperationException("Customer account is on hold.");

        var productIds = request.Items.Select(item => item.ProductId).Distinct().ToList();
        var products = await repository.GetProductsAsync(productIds, cancellationToken);

        if (products.Count != productIds.Count)
            throw new KeyNotFoundException("One or more products were not found.");

        foreach (var item in request.Items)
        {
            if (item.Qty <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            if (products[item.ProductId].Stock < item.Qty)
                throw new InvalidOperationException($"Insufficient stock for {products[item.ProductId].Name}.");
        }

        var subtotal = request.Items.Sum(item => item.Qty * products[item.ProductId].Price);
        var discountPercent = Math.Clamp(request.DiscountPct, 0, 100);
        var discount = Math.Round(subtotal * discountPercent / 100m, 2);
        var taxableAmount = subtotal - discount;

        var configuration = await repository.GetConfigurationAsync(cancellationToken) ?? new AppConfig();
        var tax = Math.Round(taxableAmount * configuration.TaxRate / 100m, 2);
        var shipping = taxableAmount >= configuration.FreeShipOver ? 0 : configuration.ShippingFee;
        var total = taxableAmount + tax + shipping;

        Order? order = null;
        await repository.ExecuteInTransactionAsync(async () =>
        {
            order = new Order
            {
                CustomerId = customer.Id,
                Subtotal = subtotal,
                DiscountPct = discountPercent,
                Discount = discount,
                Tax = tax,
                Shipping = shipping,
                Total = total
            };

            foreach (var item in request.Items)
            {
                var product = products[item.ProductId];
                product.Stock -= item.Qty;

                order.Items.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    Qty = item.Qty,
                    Price = product.Price
                });

                repository.AddStockMovement(new StockMovement
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Delta = -item.Qty,
                    Reason = "Sale",
                    Reference = "Order",
                    By = actor
                });
            }

            repository.AddAuditLog(new AuditLogEntry
            {
                User = actor,
                Action = "Created order",
                Module = "Sales",
                Target = "New order",
                Detail = $"Total {total:N2}"
            });

            await repository.AddAsync(order, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }, cancellationToken);

        return Map(order!);
    }

    public async Task<OrderDto?> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await repository.GetByIdAsync(id, cancellationToken);
        return order is null ? null : Map(order);
    }

    public async Task<IReadOnlyList<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var orders = await repository.GetAllAsync(cancellationToken);
        return orders.Select(Map).ToList();
    }

    private static OrderDto Map(Order order) =>
        new(order.Id, order.CustomerId, order.Status, order.OrderDate, order.Subtotal,
            order.Tax, order.Shipping, order.Discount, order.Total);
}
