using XeoTechErp.Api.Domain.Enums;

namespace XeoTechErp.Api.DTOs;

public record LoginRequest(string Email, string Password);
public record LoginResponse(string Token, DateTime ExpiresAt, int UserId, string DisplayName, Role Role);

public record ProductDto(int Id, string Sku, string Name, string Category, decimal Price, decimal Cost, int Stock, int ReorderLevel, int? SupplierId);
public record CreateProductRequest(string Sku, string Name, string Category, decimal Price, decimal Cost, int Stock, int ReorderLevel, int? SupplierId);

public record CustomerDto(int Id, string Company, string ContactName, string Email, string Phone, string Country, CustomerTier Tier, string PaymentTerms, decimal CreditLimit, bool OnHold);
public record CreateCustomerRequest(string Company, string ContactName, string Email, string Phone, string Country, CustomerTier Tier, string PaymentTerms, decimal CreditLimit);

public record OrderItemRequest(int ProductId, int Qty);
public record CreateOrderRequest(int CustomerId, List<OrderItemRequest> Items, decimal DiscountPct = 0);
public record OrderDto(int Id, int CustomerId, OrderStatus Status, DateTime OrderDate, decimal Subtotal, decimal Tax, decimal Shipping, decimal Discount, decimal Total);
