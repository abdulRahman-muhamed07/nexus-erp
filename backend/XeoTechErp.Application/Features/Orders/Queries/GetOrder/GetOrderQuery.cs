using XeoTechErp.Application.Abstractions.Messaging;
using XeoTechErp.Application.Features.Orders.Common;

namespace XeoTechErp.Application.Features.Orders.Queries.GetOrder;

public sealed record GetOrderQuery(int Id) : IQuery<OrderDto?>;
