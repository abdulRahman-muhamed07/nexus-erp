using XeoTechErp.Application.Common;
using XeoTechErp.Application.Common.Models;

namespace XeoTechErp.Application.Abstractions.Services;

public interface IInventoryService
{
    Task<InventorySummaryResponse> GetSummaryAsync(CancellationToken cancellationToken = default);
    Task<Result> AdjustAsync(int productId, int delta, string reason, string actor, CancellationToken cancellationToken = default);
}
