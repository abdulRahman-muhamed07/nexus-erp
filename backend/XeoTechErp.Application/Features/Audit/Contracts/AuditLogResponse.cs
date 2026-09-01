namespace XeoTechErp.Application.Contracts.Audit;

public sealed record AuditLogResponse(int Id, DateTime Time, string User, string Role, string Icon, string Action, string Module, string Target, string Detail);
