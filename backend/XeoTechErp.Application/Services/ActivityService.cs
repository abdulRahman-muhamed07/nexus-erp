using AutoMapper;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Application.Contracts.Activities;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Application.Services;

public sealed class ActivityService(IActivityRepository repository, IUnitOfWork unitOfWork, IMapper mapper) : IActivityService
{
    public async Task<IReadOnlyList<ActivityResponse>> GetRecentAsync(int take = 100, CancellationToken cancellationToken = default)
        => mapper.Map<List<ActivityResponse>>(await repository.GetRecentAsync(Math.Clamp(take, 1, 100), cancellationToken));

    public async Task<ActivityResponse> CreateAsync(CreateActivityRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Text)) throw new ArgumentException("Activity text is required.");
        var activity = new Activity { Icon = request.Icon?.Trim() ?? string.Empty, Text = request.Text.Trim(), Time = DateTime.UtcNow };
        repository.Add(activity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return mapper.Map<ActivityResponse>(activity);
    }
}
