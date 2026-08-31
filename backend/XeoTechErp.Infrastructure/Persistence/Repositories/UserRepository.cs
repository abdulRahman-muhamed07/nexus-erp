using Microsoft.EntityFrameworkCore;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(XeoTechDbContext db) : EfRepository<User>(db), IUserRepository
{
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => db.Users.SingleOrDefaultAsync(x => x.Email == email, cancellationToken);
}
