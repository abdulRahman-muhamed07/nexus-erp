using XeoTechErp.Application.Common;

namespace XeoTechErp.Application.Features.Finance.Assets;

public interface IAssetService
{
    Task<IReadOnlyList<AssetResponse>> GetAsync(CancellationToken cancellationToken = default);
    Task<Result<AssetResponse>> CreateAsync(CreateAssetRequest request, CancellationToken cancellationToken = default);
    Task<Result<AssetResponse>> DisposeAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DepreciationResponse>> GetDepreciationAsync(CancellationToken cancellationToken = default);
}
