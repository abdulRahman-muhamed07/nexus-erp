using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Application.Contracts.Orders;
using XeoTechErp.Application.CQRS;

namespace XeoTechErp.Application.Features.Orders.Queries.GetOrders;

public sealed class GetOrdersQueryHandler(IOrderRepository orders) : IQueryHandler<GetOrdersQuery, IReadOnlyList<OrderDto>>
{
    public async Task<IReadOnlyList<OrderDto>> HandleAsync(GetOrdersQuery query, CancellationToken cancellationToken = default)
    {
        var entities = await orders.GetAllAsync(cancellationToken);
        return entities
            .Select(order => new OrderDto(
                order.Id,
                order.CustomerId,
                order.Status,
                order.OrderDate,
                order.Subtotal,
                order.Tax,
                order.Shipping,
                order.Discount,
                order.Total))
            .ToList();
    }
}
