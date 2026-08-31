using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Application.Abstractions.Persistence;

public interface IAuditRepository
{
    Task<(IReadOnlyList<AuditLogEntry> Data, int Total)> GetAsync(int page, int pageSize, string? module, CancellationToken cancellationToken = default);
}
