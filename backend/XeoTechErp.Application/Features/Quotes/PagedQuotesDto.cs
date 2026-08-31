namespace XeoTechErp.Application.Features.Quotes;

public sealed record PagedQuotesDto(IReadOnlyList<QuoteDto> Data, int Page, int PageSize, int Total);
