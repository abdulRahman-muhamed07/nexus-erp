using AutoMapper;
using XeoTechErp.Application.Abstractions.Messaging;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Application.Features.Orders.Common;

namespace XeoTechErp.Application.Features.Orders.Queries.GetOrder;

public sealed class GetOrderQueryHandler(IOrderRepository orders, IMapper mapper) : IQueryHandler<GetOrderQuery, OrderDto?>
{
    public async Task<OrderDto?> HandleAsync(GetOrderQuery query, CancellationToken cancellationToken = default)
    {
        var order = await orders.GetWithItemsAsync(query.Id, cancellationToken);
        return order is null ? null : mapper.Map<OrderDto>(order);
    }
}
