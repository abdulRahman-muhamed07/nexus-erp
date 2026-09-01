using XeoTechErp.Application.Common;
using XeoTechErp.Application.Contracts.Payments;

namespace XeoTechErp.Application.Services;

public interface IPaymentService
{
    Task<IReadOnlyList<PaymentResponse>> GetByOrderAsync(int orderId, CancellationToken cancellationToken = default);
    Task<Result<PaymentSummaryResponse>> GetSummaryAsync(int orderId, CancellationToken cancellationToken = default);
    Task<Result<PaymentResponse>> CreateAsync(CreatePaymentRequest request, CancellationToken cancellationToken = default);
}
