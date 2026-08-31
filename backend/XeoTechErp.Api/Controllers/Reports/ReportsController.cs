using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XeoTechErp.Api.Data;
using XeoTechErp.Api.Models;
namespace XeoTechErp.Api.Controllers;
[ApiController,Authorize(Roles="Manager,Administrator"),Route("api/reports")]
public class ReportsController(XeoTechDbContext db):ControllerBase
{
 [HttpGet("sales-summary")] public async Task<IActionResult> SalesSummary(){var orders=await db.Orders.AsNoTracking().ToListAsync();var valid=orders.Where(x=>x.Status!=OrderStatus.Cancelled).ToList();return Ok(new{totalOrders=orders.Count,revenue=valid.Sum(x=>x.Total),averageOrderValue=valid.Count==0?0:valid.Average(x=>x.Total)});}
}
