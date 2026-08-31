using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XeoTechErp.Api.Data;
using XeoTechErp.Api.Models;

namespace XeoTechErp.Api.Controllers.Quotes;

[ApiController, Route("api/quotes"), Authorize]
public class QuotesController(XeoTechDbContext db) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Get([FromQuery] QuoteStatus? status,[FromQuery]int page=1,[FromQuery]int pageSize=20){page=Math.Max(1,page);pageSize=Math.Clamp(pageSize,1,100);var q=db.Quotes.AsNoTracking().Include(x=>x.Customer).Include(x=>x.Items);if(status.HasValue)q=q.Where(x=>x.Status==status);var total=await q.CountAsync();return Ok(new{data=await q.OrderByDescending(x=>x.Date).Skip((page-1)*pageSize).Take(pageSize).ToListAsync(),page,pageSize,total});}
    [HttpGet("{id:int}")] public async Task<IActionResult> GetById(int id)=>Ok(await db.Quotes.AsNoTracking().Include(x=>x.Customer).Include(x=>x.Items).FirstOrDefaultAsync(x=>x.Id==id)??(object)new{error="Quote not found"});
    [HttpPost] public async Task<IActionResult> Create(Quote quote){if(quote.CustomerId<=0||quote.Items.Count==0)return BadRequest(new{error="Customer and items are required."});var ids=quote.Items.Select(x=>x.ProductId).Distinct().ToList();var products=await db.Products.Where(x=>ids.Contains(x.Id)).ToDictionaryAsync(x=>x.Id);if(products.Count!=ids.Count)return BadRequest(new{error="Invalid product."});foreach(var i in quote.Items){if(i.Qty<=0||i.Price<0)return BadRequest(new{error="Invalid quantity or price."});i.Id=0;i.Name=products[i.ProductId].Name;}quote.Id=0;quote.Date=DateTime.UtcNow;quote.DiscountPct=Math.Clamp(quote.DiscountPct,0,100);quote.Subtotal=quote.Items.Sum(x=>x.Qty*x.Price);var net=quote.Subtotal*(1-quote.DiscountPct/100m);var c=await db.AppConfig.AsNoTracking().FirstOrDefaultAsync();quote.Shipping=net>=(c?.FreeShipOver??1000)?0:c?.ShippingFee??25;quote.Tax=net*(c?.TaxRate??8)/100;quote.Total=net+quote.Tax+quote.Shipping;db.Quotes.Add(quote);await db.SaveChangesAsync();return CreatedAtAction(nameof(GetById),new{id=quote.Id},quote);}
    [HttpPatch("{id:int}/status")] public async Task<IActionResult> Status(int id,QuoteStatus status){var q=await db.Quotes.FindAsync(id);if(q is null)return NotFound();q.Status=status;await db.SaveChangesAsync();return Ok(q);}
    [HttpPost("{id:int}/convert")] public async Task<IActionResult> Convert(int id){await using var tx=await db.Database.BeginTransactionAsync();var q=await db.Quotes.Include(x=>x.Items).FirstOrDefaultAsync(x=>x.Id==id);if(q is null)return NotFound();if(q.Status!=QuoteStatus.Approved)return Conflict(new{error="Quote must be approved first."});var o=new Order{CustomerId=q.CustomerId,QuoteId=q.Id,Subtotal=q.Subtotal,Tax=q.Tax,Shipping=q.Shipping,DiscountPct=q.DiscountPct,Discount=q.Subtotal*q.DiscountPct/100m,Total=q.Total};foreach(var i in q.Items)o.Items.Add(new OrderItem{ProductId=i.ProductId,Name=i.Name,Qty=i.Qty,Price=i.Price});db.Orders.Add(o);q.Status=QuoteStatus.Converted;await db.SaveChangesAsync();await tx.CommitAsync();return Ok(o);}
}