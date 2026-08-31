namespace XeoTechErp.Application.Contracts.Payments;

public sealed record PaymentSummaryResponse(int OrderId, decimal Total, decimal Paid, decimal Balance, string Status);
