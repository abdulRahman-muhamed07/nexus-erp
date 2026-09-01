namespace XeoTechErp.Application.Features.Orders.Commands.CreateOrder;

public sealed record CreateOrderRequest(
    int CustomerId,
    IReadOnlyList<OrderItemRequest> Items,
    decimal DiscountPct = 0);
