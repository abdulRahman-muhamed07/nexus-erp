using XeoTechErp.Application.Contracts.Notifications;

namespace XeoTechErp.Application.Services;

public interface INotificationService
{
    Task<IReadOnlyList<NotificationResponse>> GetAsync(bool unreadOnly, int take, CancellationToken cancellationToken = default);
    Task<bool> MarkReadAsync(int id, CancellationToken cancellationToken = default);
    Task MarkAllReadAsync(CancellationToken cancellationToken = default);
}
