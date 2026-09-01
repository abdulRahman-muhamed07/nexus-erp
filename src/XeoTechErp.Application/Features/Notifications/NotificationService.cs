using AutoMapper;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Application.Contracts.Notifications;

namespace XeoTechErp.Application.Services;

public sealed class NotificationService(INotificationRepository repository, IUnitOfWork unitOfWork, IMapper mapper) : INotificationService
{
    public async Task<IReadOnlyList<NotificationResponse>> GetAsync(bool unreadOnly, int take, CancellationToken cancellationToken = default)
        => mapper.Map<IReadOnlyList<NotificationResponse>>(await repository.GetAsync(unreadOnly, Math.Clamp(take, 1, 100), cancellationToken));

    public async Task<bool> MarkReadAsync(int id, CancellationToken cancellationToken = default)
    {
        var notification = await repository.GetByIdAsync(id, cancellationToken);
        if (notification is null) return false;
        notification.IsRead = true;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task MarkAllReadAsync(CancellationToken cancellationToken = default)
    {
        await repository.MarkAllReadAsync(cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
