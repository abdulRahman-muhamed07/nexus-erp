using AutoMapper;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Application.Common;
using XeoTechErp.Domain.Entities;
using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Application.Features.Finance.Invoices;

public sealed class InvoiceService(
    IInvoiceRepository repository,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IInvoiceService
{
    public async Task<IReadOnlyList<InvoiceResponse>> GetAsync(InvoiceStatus? status, CancellationToken cancellationToken = default)
        => mapper.Map<IReadOnlyList<InvoiceResponse>>(await repository.GetAllAsync(status, cancellationToken));

    public async Task<Result<InvoiceResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var invoice = await repository.GetByIdAsync(id, cancellationToken);
        return invoice is null
            ? Result<InvoiceResponse>.Failure("INVOICE_NOT_FOUND", "Invoice was not found.")
            : Result<InvoiceResponse>.Success(mapper.Map<InvoiceResponse>(invoice));
    }

    public async Task<Result<InvoiceResponse>> CreateFromOrderAsync(int orderId, CancellationToken cancellationToken = default)
    {
        var order = await repository.GetDeliveredOrderAsync(orderId, cancellationToken);
        if (order is null)
            return Result<InvoiceResponse>.Failure("ORDER_NOT_FOUND", "Order was not found.");
        if (order.Status != OrderStatus.Delivered)
            return Result<InvoiceResponse>.Failure("ORDER_NOT_DELIVERED", "Invoice can only be created for delivered orders.");
        if (await repository.ExistsForOrderAsync(orderId, cancellationToken))
            return Result<InvoiceResponse>.Failure("INVOICE_EXISTS", "Invoice already exists for this order.");

        var days = order.Customer.PaymentTerms switch
        {
            "Due on Receipt" => 0,
            "Net 15" => 15,
            "Net 45" => 45,
            "Net 60" => 60,
            _ => 30
        };

        var issued = DateTime.UtcNow;
        var invoice = new Invoice
        {
            OrderId = order.Id,
            CustomerId = order.CustomerId,
            CustomerName = order.Customer.Company,
            Amount = order.Total,
            Issued = issued,
            Due = issued.AddDays(days)
        };

        repository.AddInvoice(invoice);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<InvoiceResponse>.Success(mapper.Map<InvoiceResponse>(invoice));
    }

    public async Task<Result<InvoiceResponse>> PayAsync(int id, CancellationToken cancellationToken = default)
    {
        var invoice = await repository.GetByIdAsync(id, cancellationToken);
        if (invoice is null)
            return Result<InvoiceResponse>.Failure("INVOICE_NOT_FOUND", "Invoice was not found.");

        if (invoice.Status != InvoiceStatus.Paid)
        {
            invoice.Status = InvoiceStatus.Paid;
            invoice.PaidOn = DateTime.UtcNow;

            if (invoice.OrderId is int orderId)
            {
                var paid = await repository.GetOrderPaymentsAsync(orderId, cancellationToken);
                if (paid < invoice.Amount)
                {
                    repository.AddPayment(new Payment
                    {
                        OrderId = orderId,
                        Amount = invoice.Amount - paid,
                        Method = PaymentMethod.Other
                    });
                }
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result<InvoiceResponse>.Success(mapper.Map<InvoiceResponse>(invoice));
    }
}