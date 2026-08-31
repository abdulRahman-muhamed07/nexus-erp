using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Application.Common;
using XeoTechErp.Application.Contracts.Orders;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Application.Services;

public interface IOrderService
{
    Task<IReadOnlyList<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<OrderDto?> GetAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<OrderDto>> CreateAsync(CreateOrderRequest request, string actor, CancellationToken cancellationToken = default);
}

public sealed class OrderService(IOrderRepository orders, ICustomerRepository customers, IProductRepository products, IInventoryRepository inventory, IUnitOfWork unitOfWork) : IOrderService
{
    public async Task<IReadOnlyList<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default) => (await orders.GetAllAsync(cancellationToken)).Select(ToDto).ToList();
    public async Task<OrderDto?> GetAsync(int id, CancellationToken cancellationToken = default) => await orders.GetWithItemsAsync(id, cancellationToken) is { } order ? ToDto(order) : null;
    public async Task<Result<OrderDto>> CreateAsync(CreateOrderRequest request, string actor, CancellationToken cancellationToken = default)
    {
        if (request.Items is null || request.Items.Count == 0) return Result<OrderDto>.Failure("ORDER_EMPTY", "Order must contain at least one item.");
        var customer = await customers.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer is null) return Result<OrderDto>.Failure("CUSTOMER_NOT_FOUND", "Customer was not found.");
        if (customer.OnHold) return Result<OrderDto>.Failure("CUSTOMER_ON_HOLD", "Customer account is on hold.");
        var ids = request.Items.Select(x => x.ProductId).Distinct().ToList();
        var productsById = await products.GetByIdsAsync(ids, cancellationToken);
        if (productsById.Count != ids.Count) return Result<OrderDto>.Failure("PRODUCT_NOT_FOUND", "One or more products were not found.");
        foreach (var item in request.Items)
        {
            if (item.Qty <= 0) return Result<OrderDto>.Failure("INVALID_QUANTITY", "Quantity must be greater than zero.");
            if (productsById[item.ProductId].Stock < item.Qty) return Result<OrderDto>.Failure("INSUFFICIENT_STOCK", $"Insufficient stock for {productsById[item.ProductId].Name}.");
        }
        var subtotal = request.Items.Sum(x => x.Qty * productsById[x.ProductId].Price);
        var discountPct = Math.Clamp(request.DiscountPct, 0m, 100m);
        var discount = Math.Round(subtotal * discountPct / 100m, 2);
        var taxable = subtotal - discount;
        var tax = Math.Round(taxable * 0.08m, 2);
        var shipping = taxable >= 1000m ? 0m : 25m;
        var order = new Order { CustomerId = customer.Id, Subtotal = subtotal, DiscountPct = discountPct, Discount = discount, Tax = tax, Shipping = shipping, Total = taxable + tax + shipping };
        foreach (var item in request.Items)
        {
            var product = productsById[item.ProductId];
            product.Stock -= item.Qty;
            order.Items.Add(new OrderItem { ProductId = product.Id, Name = product.Name, Qty = item.Qty, Price = product.Price });
            inventory.AddMovement(new StockMovement { ProductId = product.Id, ProductName = product.Name, Delta = -item.Qty, Reason = "Sale", Reference = "Order", By = actor });
        }
        orders.Add(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<OrderDto>.Success(ToDto(order));
    }
    private static OrderDto ToDto(Order o) => new(o.Id, o.CustomerId, o.Status, o.OrderDate, o.Subtotal, o.Tax, o.Shipping, o.Discount, o.Total);
}
