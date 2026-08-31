using AutoMapper;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Application.Common;
using XeoTechErp.Application.Contracts.Audit;

namespace XeoTechErp.Application.Services;

public sealed class AuditService(IAuditRepository repository, IMapper mapper) : IAuditService
{
    public async Task<PagedResult<AuditLogResponse>> GetAsync(int page, int pageSize, string? module, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var result = await repository.GetAsync(page, pageSize, module, cancellationToken);
        return new PagedResult<AuditLogResponse>(mapper.Map<IReadOnlyList<AuditLogResponse>>(result.Data), page, pageSize, result.Total);
    }
}
