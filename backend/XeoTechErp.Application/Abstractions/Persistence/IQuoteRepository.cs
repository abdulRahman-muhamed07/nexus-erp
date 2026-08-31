using XeoTechErp.Domain.Entities;
using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Application.Abstractions.Persistence;

public interface IQuoteRepository
{
    Task<(IReadOnlyList<Quote> Data, int Total)> GetAsync(QuoteStatus? status, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Quote?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Dictionary<int, Product>> GetProductsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    Task<AppConfig?> GetConfigAsync(CancellationToken cancellationToken = default);
    void Add(Quote quote);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
