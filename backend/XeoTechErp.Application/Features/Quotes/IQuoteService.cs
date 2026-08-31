using XeoTechErp.Application.Common;
using XeoTechErp.Domain.Entities;
using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Application.Features.Quotes;

public interface IQuoteService
{
    Task<PagedQuotesDto> GetAsync(QuoteStatus? status, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<QuoteDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<QuoteDto> CreateAsync(CreateQuoteRequest request, CancellationToken cancellationToken = default);
    Task<QuoteDto> UpdateStatusAsync(int id, QuoteStatus status, CancellationToken cancellationToken = default);
    Task<Order> ConvertAsync(int id, CancellationToken cancellationToken = default);
}
