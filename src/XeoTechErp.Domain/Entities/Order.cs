using XeoTechErp.Domain.Enums;
using XeoTechErp.Domain.Exceptions;

namespace XeoTechErp.Domain.Entities;

public sealed class Order
{
    private readonly List<OrderItem> _items = new();

    public int Id { get; private set; }
    public int CustomerId { get; private set; }
    public Customer Customer { get; private set; } = null!;
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public DateTime OrderDate { get; private set; } = DateTime.UtcNow;
    public decimal Subtotal { get; private set; }
    public decimal Tax { get; private set; }
    public decimal Shipping { get; private set; }
    public decimal Total { get; private set; }
    public decimal DiscountPct { get; private set; }
    public decimal Discount { get; private set; }
    public int? QuoteId { get; private set; }
    public Quote? Quote { get; private set; }
    public int? InvoiceId { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items;
    public ICollection<Payment> Payments { get; private set; } = new List<Payment>();
    public ICollection<Return> Returns { get; private set; } = new List<Return>();

    private Order() { }

    public Order(int customerId)
    {
        if (customerId <= 0) throw new DomainRuleException("Customer is required.");
        CustomerId = customerId;
    }

    public static Order FromQuote(Quote quote)
    {
        if (quote is null) throw new ArgumentNullException(nameof(quote));
        if (quote.CustomerId <= 0) throw new DomainRuleException("Customer is required.");
        if (quote.Items.Count == 0) throw new DomainRuleException("Quote must contain at least one item.");

        var order = new Order(quote.CustomerId)
        {
            QuoteId = quote.Id,
            Tax = quote.Tax,
            Shipping = quote.Shipping,
            DiscountPct = quote.DiscountPct
        };

        foreach (var item in quote.Items)
        {
            if (item.Qty <= 0 || item.Price < 0)
                throw new DomainRuleException("Quote contains an invalid item.");

            order._items.Add(new OrderItem(item.ProductId, item.Name, item.Qty, item.Price));
        }

        order.RecalculateTotals();
        return order;
    }

    public void AddItem(Product product, int quantity)
    {
        if (product is null) throw new ArgumentNullException(nameof(product));
        product.DecreaseStock(quantity);
        _items.Add(new OrderItem(product, quantity));
        RecalculateTotals();
    }

    public void ApplyDiscount(decimal percentage)
    {
        if (percentage is < 0 or > 100) throw new DomainRuleException("Discount must be between 0 and 100 percent.");
        DiscountPct = percentage;
        RecalculateTotals();
    }

    public void SetCharges(decimal tax, decimal shipping)
    {
        if (tax < 0 || shipping < 0) throw new DomainRuleException("Tax and shipping cannot be negative.");
        Tax = tax;
        Shipping = shipping;
        RecalculateTotals();
    }

    public void Cancel()
    {
        if (Status is OrderStatus.Delivered or OrderStatus.Cancelled)
            throw new DomainRuleException("Only active orders can be cancelled.");
        Status = OrderStatus.Cancelled;
    }

    private void RecalculateTotals()
    {
        Subtotal = _items.Sum(x => x.Qty * x.Price);
        Discount = Math.Round(Subtotal * DiscountPct / 100m, 2);
        Total = Subtotal - Discount + Tax + Shipping;
    }
}
