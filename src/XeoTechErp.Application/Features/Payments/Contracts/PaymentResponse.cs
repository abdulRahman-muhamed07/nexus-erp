using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Application.Contracts.Payments;

public sealed record PaymentResponse(int Id, int OrderId, decimal Amount, PaymentMethod Method, DateTime Date);
