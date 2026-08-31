using XeoTechErp.Application.Abstractions.Messaging;
using XeoTechErp.Application.Common;
using XeoTechErp.Application.Features.Orders.Common;

namespace XeoTechErp.Application.Features.Orders.Commands.CreateOrder;

public sealed record CreateOrderCommand(CreateOrderRequest Request, string Actor) : ICommand<Result<OrderDto>>;
