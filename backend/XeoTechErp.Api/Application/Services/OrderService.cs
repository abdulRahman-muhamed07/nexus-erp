using Microsoft.EntityFrameworkCore;
using XeoTechErp.Api.Data;
using XeoTechErp.Api.DTOs;
using XeoTechErp.Api.Models;

namespace XeoTechErp.Api.Services;
public interface IOrderService { Task<OrderDto> CreateAsync(CreateOrderRequest request, string actor); Task<OrderDto?> GetAsync(int id); Task<List<OrderDto>> GetAllAsync(); }
public sealed class OrderService(XeoTechDbContext db) : IOrderService
{
    public async Task<OrderDto> CreateAsync(CreateOrderRequest r,string actor)
    {
        if(r.Items is null || r.Items.Count==0) throw new ArgumentException("Order must contain at least one item.");
        var customer=await db.Customers.FindAsync(r.CustomerId) ?? throw new KeyNotFoundException("Customer not found.");
        if(customer.OnHold) throw new InvalidOperationException("Customer account is on hold.");
        var ids=r.Items.Select(x=>x.ProductId).Distinct().ToList(); var products=await db.Products.Where(p=>ids.Contains(p.Id)).ToDictionaryAsync(p=>p.Id);
        if(products.Count!=ids.Count) throw new KeyNotFoundException("One or more products were not found.");
        foreach(var item in r.Items) if(item.Qty<=0) throw new ArgumentException("Quantity must be greater than zero."); else if(products[item.ProductId].Stock<item.Qty) throw new InvalidOperationException($"Insufficient stock for {products[item.ProductId].Name}.");
        var subtotal=r.Items.Sum(i=>i.Qty*products[i.ProductId].Price); var discount=Math.Round(subtotal*Math.Clamp(r.DiscountPct,0,100)/100m,2); var taxable=subtotal-discount;
        var config=await db.AppConfig.AsNoTracking().FirstOrDefaultAsync() ?? new AppConfig(); var tax=Math.Round(taxable*config.TaxRate/100m,2); var shipping=taxable>=config.FreeShipOver?0:config.ShippingFee; var total=taxable+tax+shipping;
        await using var tx=await db.Database.BeginTransactionAsync();
        var order=new Order{CustomerId=r.CustomerId,Subtotal=subtotal,DiscountPct=r.DiscountPct,Discount=discount,Tax=tax,Shipping=shipping,Total=total};
        foreach(var item in r.Items){var p=products[item.ProductId];p.Stock-=item.Qty;order.Items.Add(new OrderItem{ProductId=p.Id,Name=p.Name,Qty=item.Qty,Price=p.Price});db.StockMovements.Add(new StockMovement{ProductId=p.Id,ProductName=p.Name,Delta=-item.Qty,Reason="Sale",Reference="Order",By=actor});}
        db.Orders.Add(order); db.AuditLog.Add(new AuditLogEntry{User=actor,Role="",Action="Created order",Module="Sales",Target=$"Order #{order.Id}",Detail=$"Total {total:N2}"}); await db.SaveChangesAsync(); await tx.CommitAsync();
        return ToDto(order);
    }
    public async Task<OrderDto?> GetAsync(int id)=>await db.Orders.AsNoTracking().Where(o=>o.Id==id).Select(o=>new OrderDto(o.Id,o.CustomerId,o.Status,o.OrderDate,o.Subtotal,o.Tax,o.Shipping,o.Discount,o.Total)).SingleOrDefaultAsync();
    public async Task<List<OrderDto>> GetAllAsync()=>await db.Orders.AsNoTracking().OrderByDescending(o=>o.OrderDate).Select(o=>new OrderDto(o.Id,o.CustomerId,o.Status,o.OrderDate,o.Subtotal,o.Tax,o.Shipping,o.Discount,o.Total)).ToListAsync();
    static OrderDto ToDto(Order o)=>new(o.Id,o.CustomerId,o.Status,o.OrderDate,o.Subtotal,o.Tax,o.Shipping,o.Discount,o.Total);
}
