using Microsoft.EntityFrameworkCore;
using XeoTechErp.Api.Data;
using XeoTechErp.Api.Models;
namespace XeoTechErp.Api.Services;
public interface IDashboardService { Task<object> GetAsync(); }
public sealed class DashboardService(XeoTechDbContext db):IDashboardService
{
 public async Task<object> GetAsync()=>new { revenue=await db.Orders.Where(o=>o.Status!=OrderStatus.Cancelled).SumAsync(o=>(decimal?)o.Total)??0m, orders=await db.Orders.CountAsync(), customers=await db.Customers.CountAsync(), products=await db.Products.CountAsync(), lowStock=await db.Products.CountAsync(p=>p.Stock<=p.ReorderLevel) };
}
