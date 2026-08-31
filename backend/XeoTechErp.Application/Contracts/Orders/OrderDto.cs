using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Application.Contracts.Orders;

public sealed record OrderDto(int Id, int CustomerId, OrderStatus Status, DateTime OrderDate, decimal Subtotal, decimal Tax, decimal Shipping, decimal Discount, decimal Total);
