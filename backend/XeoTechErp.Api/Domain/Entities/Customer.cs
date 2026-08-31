using System.ComponentModel.DataAnnotations;
using XeoTechErp.Api.Domain.Enums;

namespace XeoTechErp.Api.Domain.Entities;

public sealed class Customer
{
    public int Id { get; set; }
    [Required, MaxLength(120)] public string Company { get; set; } = null!;
    [MaxLength(120)] public string ContactName { get; set; } = string.Empty;
    [MaxLength(120)] public string Email { get; set; } = string.Empty;
    [MaxLength(40)] public string Phone { get; set; } = string.Empty;
    [MaxLength(60)] public string Country { get; set; } = string.Empty;
    public CustomerTier Tier { get; set; } = CustomerTier.Standard;
    public DateTime Since { get; set; } = DateTime.UtcNow;
    [MaxLength(30)] public string PaymentTerms { get; set; } = "Net 30";
    public decimal CreditLimit { get; set; }
    public bool OnHold { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
