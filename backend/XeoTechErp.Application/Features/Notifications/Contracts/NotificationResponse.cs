namespace XeoTechErp.Application.Contracts.Notifications;

public sealed record NotificationResponse(int Id, string Icon, string Title, string Description, DateTime Time, bool IsRead);
