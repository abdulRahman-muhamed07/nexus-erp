namespace XeoTechErp.Application.Contracts.Returns;

public sealed record CreateReturnRequest(int OrderId, decimal Amount, string Reason);
