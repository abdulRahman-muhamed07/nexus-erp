using AutoMapper;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Application.Contracts.Orders;
using XeoTechErp.Application.CQRS;

namespace XeoTechErp.Application.Features.Orders.Queries.GetOrders;

public sealed class GetOrdersQueryHandler(IOrderRepository orders, IMapper mapper) : IQueryHandler<GetOrdersQuery, IReadOnlyList<OrderDto>>
{
    public async Task<IReadOnlyList<OrderDto>> HandleAsync(GetOrdersQuery query, CancellationToken cancellationToken = default)
    {
        var entities = await orders.GetAllAsync(cancellationToken);
        return mapper.Map<IReadOnlyList<OrderDto>>(entities);
    }
}
