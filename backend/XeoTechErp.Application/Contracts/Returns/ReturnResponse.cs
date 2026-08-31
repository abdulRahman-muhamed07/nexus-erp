namespace XeoTechErp.Application.Contracts.Returns;

public sealed record ReturnResponse(int Id, int OrderId, decimal Amount, string Reason, DateTime Date);
