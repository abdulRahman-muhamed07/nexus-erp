using Microsoft.EntityFrameworkCore;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Infrastructure.Persistence.Repositories;

public sealed class AuditRepository(XeoTechDbContext db) : IAuditRepository
{
    public async Task<(IReadOnlyList<AuditLogEntry> Data, int Total)> GetAsync(int page, int pageSize, string? module, CancellationToken cancellationToken = default)
    {
        var query = db.AuditLog.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(module)) query = query.Where(x => x.Module == module);
        var total = await query.CountAsync(cancellationToken);
        var data = await query.OrderByDescending(x => x.Time).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return (data, total);
    }
}
