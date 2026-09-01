using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Application.Features.Orders.Common;

public sealed record OrderDto(
    int Id,
    int CustomerId,
    OrderStatus Status,
    DateTime OrderDate,
    decimal Subtotal,
    decimal Tax,
    decimal Shipping,
    decimal Discount,
    decimal Total);
