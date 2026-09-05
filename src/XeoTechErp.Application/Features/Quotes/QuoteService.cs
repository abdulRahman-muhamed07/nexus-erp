using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Domain.Entities;
using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Application.Features.Quotes;

public sealed class QuoteService(IQuoteRepository repository, IOrderRepository orderRepository, IUnitOfWork unitOfWork) : IQuoteService
{
    public async Task<PagedQuotesDto> GetAsync(QuoteStatus? status, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var result = await repository.GetAsync(status, page, pageSize, cancellationToken);
        return new PagedQuotesDto(result.Data.Select(ToDto).ToList(), page, pageSize, result.Total);
    }

    public async Task<QuoteDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => ToDto(await repository.GetByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException("Quote not found."));

    public async Task<QuoteDto> CreateAsync(CreateQuoteRequest request, CancellationToken cancellationToken = default)
    {
        if (request.CustomerId <= 0 || request.Items.Count == 0)
            throw new ArgumentException("Customer and items are required.");

        var ids = request.Items.Select(x => x.ProductId).Distinct().ToList();
        var products = await repository.GetProductsAsync(ids, cancellationToken);
        if (products.Count != ids.Count)
            throw new KeyNotFoundException("One or more products were not found.");

        var quote = new Quote
        {
            CustomerId = request.CustomerId,
            Date = DateTime.UtcNow,
            DiscountPct = Math.Clamp(request.DiscountPct, 0m, 100m)
        };

        foreach (var item in request.Items)
        {
            if (item.Qty <= 0 || item.Price < 0)
                throw new ArgumentException("Invalid quantity or price.");
            quote.Items.Add(new QuoteItem
            {
                ProductId = item.ProductId,
                Name = products[item.ProductId].Name,
                Qty = item.Qty,
                Price = item.Price
            });
        }

        quote.Subtotal = quote.Items.Sum(x => x.Qty * x.Price);
        var net = quote.Subtotal * (1 - quote.DiscountPct / 100m);
        var config = await repository.GetConfigAsync(cancellationToken);
        quote.Shipping = net >= (config?.FreeShipOver ?? 1000m) ? 0m : config?.ShippingFee ?? 25m;
        quote.Tax = net * (config?.TaxRate ?? 8m) / 100m;
        quote.Total = net + quote.Tax + quote.Shipping;

        repository.Add(quote);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ToDto(quote);
    }

    public async Task<QuoteDto> UpdateStatusAsync(int id, QuoteStatus status, CancellationToken cancellationToken = default)
    {
        var quote = await repository.GetByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException("Quote not found.");
        quote.Status = status;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ToDto(quote);
    }

    public async Task<Order> ConvertAsync(int id, CancellationToken cancellationToken = default)
    {
        var quote = await repository.GetByIdAsync(id, cancellationToken) ?? throw new KeyNotFoundException("Quote not found.");
        if (quote.Status != QuoteStatus.Approved)
            throw new InvalidOperationException("Quote must be approved first.");

        var order = Order.FromQuote(quote);
        quote.Status = QuoteStatus.Converted;

        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            orderRepository.Add(order);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }, cancellationToken);

        return order;
    }

    private static QuoteDto ToDto(Quote quote) => new(
        quote.Id,
        quote.CustomerId,
        quote.Status,
        quote.Date,
        quote.Subtotal,
        quote.Tax,
        quote.Shipping,
        quote.Total,
        quote.DiscountPct);
}
