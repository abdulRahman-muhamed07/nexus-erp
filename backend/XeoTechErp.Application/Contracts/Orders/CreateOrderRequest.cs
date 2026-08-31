namespace XeoTechErp.Application.Contracts.Orders;

public sealed record CreateOrderRequest(int CustomerId, IReadOnlyList<OrderItemRequest> Items, decimal DiscountPct = 0);
