using XeoTechErp.Application.Contracts.Activities;

namespace XeoTechErp.Application.Features.Activities;

public interface IActivityService
{
    Task<IReadOnlyList<ActivityResponse>> GetRecentAsync(int take = 100, CancellationToken cancellationToken = default);
    Task<ActivityResponse> CreateAsync(CreateActivityRequest request, CancellationToken cancellationToken = default);
}
