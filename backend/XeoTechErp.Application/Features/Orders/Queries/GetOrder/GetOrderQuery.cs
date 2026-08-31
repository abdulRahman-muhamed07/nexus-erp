using XeoTechErp.Application.Contracts.Orders;
using XeoTechErp.Application.CQRS;

namespace XeoTechErp.Application.Features.Orders.Queries.GetOrder;

public sealed record GetOrderQuery(int Id) : IQuery<OrderDto?>;
