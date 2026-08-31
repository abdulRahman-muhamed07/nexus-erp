using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Application.Abstractions.Authentication;

public interface IPasswordVerifier
{
    bool Verify(User user, string password);
}
