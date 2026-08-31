using Microsoft.EntityFrameworkCore;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Infrastructure.Persistence.Repositories;

public sealed class AppConfigRepository(XeoTechDbContext db) : IAppConfigRepository
{
    public Task<AppConfig?> GetAsync(CancellationToken cancellationToken = default)
        => db.AppConfig.AsNoTracking().FirstOrDefaultAsync(cancellationToken);
}
