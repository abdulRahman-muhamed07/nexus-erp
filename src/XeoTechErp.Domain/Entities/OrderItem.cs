using XeoTechErp.Domain.Exceptions;

namespace XeoTechErp.Domain.Entities;

public sealed class OrderItem
{
    public int Id { get; private set; }
    public int OrderId { get; private set; }
    public Order Order { get; private set; } = null!;
    public int ProductId { get; private set; }
    public Product Product { get; private set; } = null!;
    public string Name { get; private set; } = string.Empty;
    public int Qty { get; private set; }
    public decimal Price { get; private set; }

    private OrderItem() { }

    internal OrderItem(Product product, int quantity)
    {
        if (quantity <= 0) throw new DomainRuleException("Quantity must be greater than zero.");
        ProductId = product.Id;
        Product = product;
        Name = product.Name;
        Qty = quantity;
        Price = product.Price;
    }
}
