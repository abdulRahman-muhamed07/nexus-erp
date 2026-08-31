using XeoTechErp.Application.Contracts.Audit;
using XeoTechErp.Application.Common;

namespace XeoTechErp.Application.Services;

public interface IAuditService
{
    Task<PagedResult<AuditLogResponse>> GetAsync(int page, int pageSize, string? module, CancellationToken cancellationToken = default);
}
