namespace XeoTechErp.Application.Features.Quotes;

public sealed record CreateQuoteRequest(int CustomerId, IReadOnlyList<QuoteItemRequest> Items, decimal DiscountPct);
