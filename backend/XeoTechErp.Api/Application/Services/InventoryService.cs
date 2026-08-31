using Microsoft.EntityFrameworkCore;
using XeoTechErp.Api.Data;
using XeoTechErp.Api.Models;
namespace XeoTechErp.Api.Services;
public interface IInventoryService { Task<object> GetSummaryAsync(); Task<bool> AdjustAsync(int productId,int delta,string reason,string actor); }
public sealed class InventoryService(XeoTechDbContext db):IInventoryService
{
 public async Task<object> GetSummaryAsync()=>await db.Products.AsNoTracking().GroupBy(_=>1).Select(g=>new{products=g.Count(),units=g.Sum(x=>x.Stock),inventoryValue=g.Sum(x=>x.Stock*x.Cost),lowStock=g.Count(x=>x.Stock<=x.ReorderLevel)}).FirstOrDefaultAsync()??new{products=0,units=0,inventoryValue=0m,lowStock=0};
 public async Task<bool> AdjustAsync(int productId,int delta,string reason,string actor){var p=await db.Products.FindAsync(productId);if(p is null)return false;if(p.Stock+delta<0)throw new InvalidOperationException("Stock cannot become negative.");p.Stock+=delta;db.StockMovements.Add(new StockMovement{ProductId=p.Id,ProductName=p.Name,Delta=delta,Reason=reason,By=actor});await db.SaveChangesAsync();return true;}
}
