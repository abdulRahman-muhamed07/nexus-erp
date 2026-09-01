using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Domain.Entities;

public sealed class Customer
{
    public int Id { get; set; }
    public string Company { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public CustomerTier Tier { get; set; } = CustomerTier.Standard;
    public DateTime Since { get; set; } = DateTime.UtcNow;
    public string PaymentTerms { get; set; } = "Net 30";
    public decimal CreditLimit { get; set; }
    public bool OnHold { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
