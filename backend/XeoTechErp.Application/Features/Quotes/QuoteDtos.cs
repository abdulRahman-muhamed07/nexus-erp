using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Application.Features.Quotes;

public sealed record QuoteItemRequest(int ProductId, int Qty, decimal Price);
public sealed record CreateQuoteRequest(int CustomerId, IReadOnlyList<QuoteItemRequest> Items, decimal DiscountPct);
public sealed record QuoteDto(int Id, int CustomerId, QuoteStatus Status, DateTime Date, decimal Subtotal, decimal Tax, decimal Shipping, decimal Total, decimal DiscountPct);
public sealed record PagedQuotesDto(IReadOnlyList<QuoteDto> Data, int Page, int PageSize, int Total);
