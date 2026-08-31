using System.ComponentModel.DataAnnotations;

namespace XeoTechErp.Api.Domain.Entities;

public sealed class Product
{
    public int Id { get; set; }
    [Required, MaxLength(40)] public string Sku { get; set; } = null!;
    [Required, MaxLength(120)] public string Name { get; set; } = null!;
    [MaxLength(60)] public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Cost { get; set; }
    public int Stock { get; set; }
    public int ReorderLevel { get; set; }
    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
}
