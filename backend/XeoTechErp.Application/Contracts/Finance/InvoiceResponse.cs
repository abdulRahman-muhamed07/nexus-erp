using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Application.Contracts.Finance;

public sealed record InvoiceResponse(int Id, int? OrderId, int? CustomerId, string CustomerName, decimal Amount, DateTime Issued, DateTime Due, InvoiceStatus Status, DateTime? PaidOn);
