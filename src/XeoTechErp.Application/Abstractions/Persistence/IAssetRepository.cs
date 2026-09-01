using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Application.Abstractions.Persistence;

public interface IAssetRepository
{
    Task<IReadOnlyList<Asset>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Asset?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    void Add(Asset asset);
}