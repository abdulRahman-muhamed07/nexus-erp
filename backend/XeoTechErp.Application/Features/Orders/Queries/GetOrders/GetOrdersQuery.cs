using XeoTechErp.Application.Contracts.Orders;
using XeoTechErp.Application.CQRS;

namespace XeoTechErp.Application.Features.Orders.Queries.GetOrders;

public sealed record GetOrdersQuery : IQuery<IReadOnlyList<OrderDto>>;
