using System.ComponentModel.DataAnnotations;

namespace XeoTechErp.Api.Models;

/// <summary>System login account. Mirrors the demo accounts used by the front-end.</summary>
public class User
{
    public int Id { get; set; }

    [Required, MaxLength(120)]
    public string Email { get; set; } = null!;

    /// <summary>Stored hash only, never plain-text.</summary>
    [Required, MaxLength(255)]
    public string PasswordHash { get; set; } = null!;

    public Role Role { get; set; } = Role.Viewer;

    [MaxLength(120)]
    public string DisplayName { get; set; } = "";
}

/// <summary>Inventory item. Weighted-average unit cost is recalculated on stock-in.</summary>
public class Product
{
    public int Id { get; set; }

    [Required, MaxLength(40)]
    public string Sku { get; set; } = null!;

    [Required, MaxLength(120)]
    public string Name { get; set; } = null!;

    [MaxLength(60)]
    public string Category { get; set; } = "";

    public decimal Price { get; set; }

    public decimal Cost { get; set; }

    public int Stock { get; set; }

    public int ReorderLevel { get; set; }

    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
}

/// <summary>CRM account with payment terms and a credit limit that gates new orders.</summary>
public class Customer
{
    public int Id { get; set; }

    [Required, MaxLength(120)]
    public string Company { get; set; } = null!;

    [MaxLength(120)]
    public string ContactName { get; set; } = "";

    [MaxLength(120)]
    public string Email { get; set; } = "";

    [MaxLength(40)]
    public string Phone { get; set; } = "";

    [MaxLength(60)]
    public string Country { get; set; } = "";

    public CustomerTier Tier { get; set; } = CustomerTier.Standard;

    public DateTime Since { get; set; } = DateTime.UtcNow;

    /// <summary>Due on Receipt / Net 15 / Net 30 / Net 45 / Net 60.</summary>
    [MaxLength(30)]
    public string PaymentTerms { get; set; } = "Net 30";

    public decimal CreditLimit { get; set; }

    public bool OnHold { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}

public class Supplier
{
    public int Id { get; set; }

    [Required, MaxLength(120)]
    public string Name { get; set; } = null!;

    [MaxLength(120)]
    public string Contact { get; set; } = "";

    [MaxLength(60)]
    public string Country { get; set; } = "";

    public double Rating { get; set; }

    [MaxLength(120)]
    public string Email { get; set; } = "";

    [MaxLength(40)]
    public string Phone { get; set; } = "";

    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
}

public class Employee
{
    public int Id { get; set; }

    [Required, MaxLength(120)]
    public string Name { get; set; } = null!;

    [MaxLength(120)]
    public string JobTitle { get; set; } = "";

    [MaxLength(60)]
    public string Department { get; set; } = "";

    [MaxLength(120)]
    public string Email { get; set; } = "";

    public decimal Salary { get; set; }

    public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;

    public DateTime HireDate { get; set; } = DateTime.UtcNow;
}

public class Order
{
    public int Id { get; set; }

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public decimal Subtotal { get; set; }

    public decimal Tax { get; set; }

    public decimal Shipping { get; set; }

    public decimal Total { get; set; }

    public decimal DiscountPct { get; set; }
    public decimal Discount { get; set; }

    public int? QuoteId { get; set; }
    public Quote? Quote { get; set; }

    public int? InvoiceId { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<Return> Returns { get; set; } = new List<Return>();
}

public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public int ProductId { get; set; }

    [MaxLength(120)]
    public string Name { get; set; } = "";

    public int Qty { get; set; }

    public decimal Price { get; set; }
}

public class Quote
{
    public int Id { get; set; }

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public QuoteStatus Status { get; set; } = QuoteStatus.Draft;

    public DateTime Date { get; set; } = DateTime.UtcNow;

    public decimal Subtotal { get; set; }

    public decimal Tax { get; set; }

    public decimal Shipping { get; set; }

    public decimal Total { get; set; }

    public decimal DiscountPct { get; set; }

    public ICollection<QuoteItem> Items { get; set; } = new List<QuoteItem>();
}

public class QuoteItem
{
    public int Id { get; set; }

    public int QuoteId { get; set; }
    public Quote Quote { get; set; } = null!;

    public int ProductId { get; set; }

    [MaxLength(120)]
    public string Name { get; set; } = "";

    public int Qty { get; set; }

    public decimal Price { get; set; }
}

public class Payment
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public decimal Amount { get; set; }

    public PaymentMethod Method { get; set; } = PaymentMethod.Other;

    public DateTime Date { get; set; } = DateTime.UtcNow;
}

public class Return
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public decimal Amount { get; set; }

    [MaxLength(200)]
    public string Reason { get; set; } = "";

    public DateTime Date { get; set; } = DateTime.UtcNow;
}

public class Invoice
{
    public int Id { get; set; }

    public int? OrderId { get; set; }
    public Order? Order { get; set; }

    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    [MaxLength(120)]
    public string CustomerName { get; set; } = "";

    public decimal Amount { get; set; }

    public DateTime Issued { get; set; } = DateTime.UtcNow;

    /// <summary>Derived from the customer's payment terms.</summary>
    public DateTime Due { get; set; }

    public InvoiceStatus Status { get; set; } = InvoiceStatus.Pending;

    public DateTime? PaidOn { get; set; }
}

public class PurchaseOrder
{
    public int Id { get; set; }

    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;

    public int ProductId { get; set; }

    [MaxLength(120)]
    public string ProductName { get; set; } = "";

    public int Qty { get; set; }

    public decimal Cost { get; set; }

    public PoStatus Status { get; set; } = PoStatus.Pending;

    public DateTime Eta { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
}

public class StockMovement
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    [MaxLength(120)]
    public string ProductName { get; set; } = "";

    public int Delta { get; set; }

    /// <summary>Sale / Cancellation / Order Edit / Manual Adjustment / PO Received / Return / Opening Stock.</summary>
    [MaxLength(40)]
    public string Reason { get; set; } = "Adjustment";

    [MaxLength(40)]
    public string? Reference { get; set; }

    [MaxLength(120)]
    public string By { get; set; } = "";

    public DateTime Time { get; set; } = DateTime.UtcNow;
}

public class Asset
{
    public int Id { get; set; }

    [Required, MaxLength(160)]
    public string Name { get; set; } = null!;

    [MaxLength(60)]
    public string Category { get; set; } = "";

    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

    public decimal Cost { get; set; }

    public decimal Salvage { get; set; }

    public int UsefulLifeYears { get; set; } = 5;

    public AssetStatus Status { get; set; } = AssetStatus.InService;

    public DateTime? DisposedOn { get; set; }
}

public class Budget
{
    public int Id { get; set; }

    [Required, MaxLength(60)]
    public string Category { get; set; } = null!;

    public decimal MonthlyAmount { get; set; }
}

/// <summary>Single-row settings: billing defaults plus free-shipping threshold.</summary>
public class AppConfig
{
    public int Id { get; set; }

    public decimal TaxRate { get; set; } = 8m;

    public decimal ShippingFee { get; set; } = 25m;

    public decimal FreeShipOver { get; set; } = 1000m;
}

public class AuditLogEntry
{
    public int Id { get; set; }

    public DateTime Time { get; set; } = DateTime.UtcNow;

    [MaxLength(120)]
    public string User { get; set; } = "";

    [MaxLength(30)]
    public string Role { get; set; } = "";

    [MaxLength(60)]
    public string Icon { get; set; } = "";

    [MaxLength(120)]
    public string Action { get; set; } = "";

    [MaxLength(60)]
    public string Module { get; set; } = "";

    [MaxLength(160)]
    public string Target { get; set; } = "";

    [MaxLength(500)]
    public string Detail { get; set; } = "";
}

public class Notification
{
    public int Id { get; set; }

    [MaxLength(60)]
    public string Icon { get; set; } = "";

    [MaxLength(160)]
    public string Title { get; set; } = "";

    [MaxLength(400)]
    public string Description { get; set; } = "";

    public DateTime Time { get; set; } = DateTime.UtcNow;

    public bool IsRead { get; set; }
}

public class Activity
{
    public int Id { get; set; }

    [MaxLength(60)]
    public string Icon { get; set; } = "";

    [MaxLength(400)]
    public string Text { get; set; } = "";

    public DateTime Time { get; set; } = DateTime.UtcNow;
}
