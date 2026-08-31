using XeoTechErp.Application.Common;

namespace XeoTechErp.Application.Services;

public interface IInventoryService
{
    Task<object> GetSummaryAsync(CancellationToken cancellationToken = default);
    Task<Result> AdjustAsync(int productId, int delta, string reason, string actor, CancellationToken cancellationToken = default);
}
