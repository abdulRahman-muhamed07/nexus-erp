using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Application.Contracts.Payments;

public sealed record CreatePaymentRequest(int OrderId, decimal Amount, PaymentMethod Method);
