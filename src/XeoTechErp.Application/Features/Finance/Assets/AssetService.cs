using AutoMapper;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Application.Common;
using XeoTechErp.Domain.Entities;
using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Application.Features.Finance.Assets;

public sealed class AssetService(
    IFinanceRepository repository,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IAssetService
{
    public async Task<IReadOnlyList<AssetResponse>> GetAsync(CancellationToken cancellationToken = default)
        => mapper.Map<IReadOnlyList<AssetResponse>>(await repository.GetAssetsAsync(cancellationToken));

    public async Task<Result<AssetResponse>> CreateAsync(CreateAssetRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || request.Cost < 0 || request.Salvage < 0 || request.Salvage > request.Cost || request.UsefulLifeYears <= 0)
            return Result<AssetResponse>.Failure("ASSET_INVALID", "Invalid asset data.");

        var asset = new Asset
        {
            Name = request.Name.Trim(),
            Category = request.Category?.Trim() ?? string.Empty,
            PurchaseDate = request.PurchaseDate == default ? DateTime.UtcNow : request.PurchaseDate,
            Cost = request.Cost,
            Salvage = request.Salvage,
            UsefulLifeYears = request.UsefulLifeYears,
            Status = AssetStatus.InService
        };

        repository.AddAsset(asset);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<AssetResponse>.Success(mapper.Map<AssetResponse>(asset));
    }

    public async Task<Result<AssetResponse>> DisposeAsync(int id, CancellationToken cancellationToken = default)
    {
        var asset = await repository.GetAssetAsync(id, cancellationToken);
        if (asset is null)
            return Result<AssetResponse>.Failure("ASSET_NOT_FOUND", "Asset was not found.");
        if (asset.Status == AssetStatus.Disposed)
            return Result<AssetResponse>.Failure("ASSET_ALREADY_DISPOSED", "Asset is already disposed.");

        asset.Status = AssetStatus.Disposed;
        asset.DisposedOn = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<AssetResponse>.Success(mapper.Map<AssetResponse>(asset));
    }

    public async Task<IReadOnlyList<DepreciationResponse>> GetDepreciationAsync(CancellationToken cancellationToken = default)
    {
        var assets = await repository.GetAssetsAsync(cancellationToken);
        return assets
            .Where(x => x.Status == AssetStatus.InService)
            .Select(x => new DepreciationResponse(
                x.Id,
                x.Name,
                Math.Round((x.Cost - x.Salvage) / (x.UsefulLifeYears * 12m), 2),
                Math.Round((x.Cost - x.Salvage) / x.UsefulLifeYears, 2)))
            .ToList();
    }
}
