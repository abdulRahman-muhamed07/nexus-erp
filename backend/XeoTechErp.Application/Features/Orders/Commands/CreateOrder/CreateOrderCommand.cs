using XeoTechErp.Application.Contracts.Orders;
using XeoTechErp.Application.CQRS;
using XeoTechErp.Application.Common;

namespace XeoTechErp.Application.Features.Orders.Commands.CreateOrder;

public sealed record CreateOrderCommand(CreateOrderRequest Request, string Actor) : ICommand<Result<OrderDto>>;
