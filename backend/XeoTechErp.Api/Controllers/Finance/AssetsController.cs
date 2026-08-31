using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using Microsoft.EntityFrameworkCore;using XeoTechErp.Api.Data;using XeoTechErp.Api.Models;
namespace XeoTechErp.Api.Controllers.Finance;
[ApiController,Route("api/assets"),Authorize]
public class AssetsController(XeoTechDbContext db):ControllerBase{
[HttpGet]public async Task<IActionResult> Get()=>Ok(await db.Assets.AsNoTracking().OrderByDescending(x=>x.PurchaseDate).ToListAsync());
[HttpPost]public async Task<IActionResult> Create(Asset a){if(string.IsNullOrWhiteSpace(a.Name)||a.Cost<0||a.Salvage<0||a.Salvage>a.Cost||a.UsefulLifeYears<=0)return BadRequest(new{error="Invalid asset data."});a.Id=0;db.Assets.Add(a);await db.SaveChangesAsync();return Created($"/api/assets/{a.Id}",a);}
[HttpPost("{id:int}/dispose")]public async Task<IActionResult> Dispose(int id){var a=await db.Assets.FindAsync(id);if(a is null)return NotFound();if(a.Status==AssetStatus.Disposed)return Conflict(new{error="Asset already disposed."});a.Status=AssetStatus.Disposed;a.DisposedOn=DateTime.UtcNow;await db.SaveChangesAsync();return Ok(a);}
[HttpGet("depreciation")]public async Task<IActionResult> Depreciation(){var assets=await db.Assets.AsNoTracking().Where(x=>x.Status==AssetStatus.InService).ToListAsync();return Ok(assets.Select(x=>new{id=x.Id,name=x.Name,monthly=(x.Cost-x.Salvage)/(x.UsefulLifeYears*12m),annual=(x.Cost-x.Salvage)/x.UsefulLifeYears}));}
}