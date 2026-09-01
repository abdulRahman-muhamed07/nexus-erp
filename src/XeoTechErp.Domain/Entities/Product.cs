namespace XeoTechErp.Domain.Entities;

using XeoTechErp.Domain.Exceptions;

public sealed class Product
{
    public int Id { get; private set; }
    public string Sku { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public decimal Cost { get; private set; }
    public int Stock { get; private set; }
    public int ReorderLevel { get; private set; }
    public int? SupplierId { get; private set; }
    public Supplier? Supplier { get; private set; }
    public ICollection<OrderItem> OrderItems { get; private set; } = new List<OrderItem>();

    private Product() { }

    public Product(string sku, string name, decimal price, decimal cost, int stock, int reorderLevel, string? category = null, int? supplierId = null)
    {
        SetDetails(sku, name, price, cost, reorderLevel, category, supplierId);
        if (stock < 0) throw new DomainRuleException("Stock cannot be negative.");
        Stock = stock;
    }

    public void SetDetails(string sku, string name, decimal price, decimal cost, int reorderLevel, string? category, int? supplierId)
    {
        if (string.IsNullOrWhiteSpace(sku)) throw new DomainRuleException("SKU is required.");
        if (string.IsNullOrWhiteSpace(name)) throw new DomainRuleException("Product name is required.");
        if (price < 0) throw new DomainRuleException("Price cannot be negative.");
        if (cost < 0) throw new DomainRuleException("Cost cannot be negative.");
        if (reorderLevel < 0) throw new DomainRuleException("Reorder level cannot be negative.");

        Sku = sku.Trim();
        Name = name.Trim();
        Category = category?.Trim() ?? string.Empty;
        Price = price;
        Cost = cost;
        ReorderLevel = reorderLevel;
        SupplierId = supplierId;
    }

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0) throw new DomainRuleException("Quantity must be greater than zero.");
        Stock += quantity;
    }

    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0) throw new DomainRuleException("Quantity must be greater than zero.");
        if (Stock < quantity) throw new DomainRuleException($"Insufficient stock for {Name}.");
        Stock -= quantity;
    }
}
