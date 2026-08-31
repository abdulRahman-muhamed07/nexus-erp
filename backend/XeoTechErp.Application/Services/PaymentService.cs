using AutoMapper;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Application.Common;
using XeoTechErp.Application.Contracts.Payments;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Application.Services;

public sealed class PaymentService(
    IPaymentRepository repository,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IPaymentService
{
    public async Task<IReadOnlyList<PaymentResponse>> GetByOrderAsync(
        int orderId,
        CancellationToken cancellationToken = default)
    {
        var payments = await repository.GetByOrderAsync(orderId, cancellationToken);
        return mapper.Map<IReadOnlyList<PaymentResponse>>(payments);
    }

    public async Task<Result<PaymentSummaryResponse>> GetSummaryAsync(
        int orderId,
        CancellationToken cancellationToken = default)
    {
        var order = await repository.GetOrderAsync(orderId, cancellationToken);
        if (order is null)
            return Result<PaymentSummaryResponse>.Failure("ORDER_NOT_FOUND", "Order was not found.");

        var paid = await repository.GetPaidAmountAsync(orderId, cancellationToken);
        var balance = Math.Max(0m, order.Total - paid);
        var status = paid <= 0m
            ? "Unpaid"
            : paid < order.Total
                ? "Partially Paid"
                : "Paid";

        return Result<PaymentSummaryResponse>.Success(
            new PaymentSummaryResponse(order.Id, order.Total, paid, balance, status));
    }

    public async Task<Result<PaymentResponse>> CreateAsync(
        CreatePaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.OrderId <= 0)
            return Result<PaymentResponse>.Failure("INVALID_ORDER", "Order is required.");

        if (request.Amount <= 0m)
            return Result<PaymentResponse>.Failure("INVALID_AMOUNT", "Payment amount must be greater than zero.");

        var order = await repository.GetOrderAsync(request.OrderId, cancellationToken);
        if (order is null)
            return Result<PaymentResponse>.Failure("ORDER_NOT_FOUND", "Order was not found.");

        var paid = await repository.GetPaidAmountAsync(request.OrderId, cancellationToken);
        var balance = order.Total - paid;

        if (request.Amount > balance)
        {
            return Result<PaymentResponse>.Failure(
                "PAYMENT_EXCEEDS_BALANCE",
                $"Payment exceeds the remaining order balance of {Math.Max(0m, balance):0.00}.");
        }

        var payment = new Payment
        {
            OrderId = request.OrderId,
            Amount = request.Amount,
            Method = request.Method,
            Date = DateTime.UtcNow
        };

        repository.Add(payment);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<PaymentResponse>.Success(mapper.Map<PaymentResponse>(payment));
    }
}
