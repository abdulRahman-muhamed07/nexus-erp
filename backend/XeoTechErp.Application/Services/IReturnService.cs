using XeoTechErp.Application.Common;
using XeoTechErp.Application.Contracts.Returns;

namespace XeoTechErp.Application.Services;

public interface IReturnService
{
    Task<IReadOnlyList<ReturnResponse>> GetAsync(CancellationToken cancellationToken = default);
    Task<Result<ReturnResponse>> CreateAsync(CreateReturnRequest request, CancellationToken cancellationToken = default);
}
