using Microsoft.EntityFrameworkCore;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Domain.Entities;
using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Infrastructure.Persistence.Repositories;

public sealed class QuoteRepository(XeoTechDbContext db) : IQuoteRepository
{
    public async Task<(IReadOnlyList<Quote> Data, int Total)> GetAsync(QuoteStatus? status, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = db.Quotes.AsNoTracking().Include(x => x.Customer).Include(x => x.Items).AsQueryable();
        if (status.HasValue) query = query.Where(x => x.Status == status);
        var total = await query.CountAsync(cancellationToken);
        var data = await query.OrderByDescending(x => x.Date).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return (data, total);
    }

    public Task<Quote?> GetByIdAsync(int id, CancellationToken cancellationToken = default) => db.Quotes.Include(x => x.Customer).Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    public Task<Dictionary<int, Product>> GetProductsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default) => db.Products.Where(x => ids.Contains(x.Id)).ToDictionaryAsync(x => x.Id, cancellationToken);
    public Task<AppConfig?> GetConfigAsync(CancellationToken cancellationToken = default) => db.AppConfig.AsNoTracking().FirstOrDefaultAsync(cancellationToken);
    public void Add(Quote quote) => db.Quotes.Add(quote);
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => db.SaveChangesAsync(cancellationToken);
}
