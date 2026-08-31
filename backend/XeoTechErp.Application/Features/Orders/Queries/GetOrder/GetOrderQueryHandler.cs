using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Application.Contracts.Orders;
using XeoTechErp.Application.CQRS;

namespace XeoTechErp.Application.Features.Orders.Queries.GetOrder;

public sealed class GetOrderQueryHandler(IOrderRepository orders) : IQueryHandler<GetOrderQuery, OrderDto?>
{
    public async Task<OrderDto?> HandleAsync(GetOrderQuery query, CancellationToken cancellationToken = default)
    {
        var order = await orders.GetWithItemsAsync(query.Id, cancellationToken);
        return order is null
            ? null
            : new OrderDto(order.Id, order.CustomerId, order.Status, order.OrderDate, order.Subtotal, order.Tax, order.Shipping, order.Discount, order.Total);
    }
}
