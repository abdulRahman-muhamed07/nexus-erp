using XeoTechErp.Api.Domain.Entities;

namespace XeoTechErp.Api.Application.Abstractions;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}