using XeoTechErp.Application.Common;
using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Application.Features.Finance.Invoices;

public interface IInvoiceService
{
    Task<IReadOnlyList<InvoiceResponse>> GetAsync(InvoiceStatus? status, CancellationToken cancellationToken = default);
    Task<Result<InvoiceResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<InvoiceResponse>> CreateFromOrderAsync(int orderId, CancellationToken cancellationToken = default);
    Task<Result<InvoiceResponse>> PayAsync(int id, CancellationToken cancellationToken = default);
}
