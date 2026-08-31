using Microsoft.EntityFrameworkCore;
using XeoTechErp.Api.Application.Abstractions;
using XeoTechErp.Api.Domain.Entities;

namespace XeoTechErp.Api.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(XeoTechDbContext db) : IUserRepository
{
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        db.Users.SingleOrDefaultAsync(user => user.Email == email, cancellationToken);
}