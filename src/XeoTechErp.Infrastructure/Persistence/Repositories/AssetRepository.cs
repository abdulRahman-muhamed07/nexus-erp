using Microsoft.EntityFrameworkCore;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Infrastructure.Persistence.Repositories;

public sealed class AssetRepository(XeoTechDbContext db) : IAssetRepository
{
    public async Task<IReadOnlyList<Asset>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await db.Assets.AsNoTracking().OrderByDescending(x => x.PurchaseDate).ToListAsync(cancellationToken);

    public Task<Asset?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.Assets.FindAsync([id], cancellationToken).AsTask();

    public void Add(Asset asset) => db.Assets.Add(asset);
}