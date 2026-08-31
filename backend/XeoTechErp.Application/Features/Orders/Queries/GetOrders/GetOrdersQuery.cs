using XeoTechErp.Application.Abstractions.Messaging;
using XeoTechErp.Application.Features.Orders.Common;

namespace XeoTechErp.Application.Features.Orders.Queries.GetOrders;

public sealed record GetOrdersQuery : IQuery<IReadOnlyList<OrderDto>>;
