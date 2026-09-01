using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Application.Features.Quotes;

public sealed record QuoteDto(int Id, int CustomerId, QuoteStatus Status, DateTime Date, decimal Subtotal, decimal Tax, decimal Shipping, decimal Total, decimal DiscountPct);
