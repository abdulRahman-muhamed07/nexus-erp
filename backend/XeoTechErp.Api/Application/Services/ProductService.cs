using Microsoft.EntityFrameworkCore;
using XeoTechErp.Api.Data;
using XeoTechErp.Api.DTOs;
using XeoTechErp.Api.Models;

namespace XeoTechErp.Api.Services;
public interface IProductService { Task<List<ProductDto>> GetAsync(string? search); Task<ProductDto?> GetAsync(int id); Task<ProductDto> CreateAsync(CreateProductRequest r); Task<bool> DeleteAsync(int id); }
public sealed class ProductService(XeoTechDbContext db) : IProductService
{
    public async Task<List<ProductDto>> GetAsync(string? search) => await db.Products.AsNoTracking().Where(p => string.IsNullOrWhiteSpace(search) || p.Name.Contains(search!) || p.Sku.Contains(search!)).OrderBy(p => p.Name).Select(p => new ProductDto(p.Id,p.Sku,p.Name,p.Category,p.Price,p.Cost,p.Stock,p.ReorderLevel,p.SupplierId)).ToListAsync();
    public async Task<ProductDto?> GetAsync(int id) => await db.Products.AsNoTracking().Where(p=>p.Id==id).Select(p=>new ProductDto(p.Id,p.Sku,p.Name,p.Category,p.Price,p.Cost,p.Stock,p.ReorderLevel,p.SupplierId)).SingleOrDefaultAsync();
    public async Task<ProductDto> CreateAsync(CreateProductRequest r)
    {
        if (await db.Products.AnyAsync(p=>p.Sku==r.Sku)) throw new InvalidOperationException("SKU already exists.");
        var p=new Product{Sku=r.Sku.Trim(),Name=r.Name.Trim(),Category=r.Category?.Trim()??"",Price=r.Price,Cost=r.Cost,Stock=r.Stock,ReorderLevel=r.ReorderLevel,SupplierId=r.SupplierId};
        db.Products.Add(p); await db.SaveChangesAsync(); return new ProductDto(p.Id,p.Sku,p.Name,p.Category,p.Price,p.Cost,p.Stock,p.ReorderLevel,p.SupplierId);
    }
    public async Task<bool> DeleteAsync(int id){var p=await db.Products.FindAsync(id);if(p is null)return false;db.Products.Remove(p);await db.SaveChangesAsync();return true;}
}
