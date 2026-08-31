using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using Microsoft.EntityFrameworkCore;using XeoTechErp.Api.Data;using XeoTechErp.Api.Models;
namespace XeoTechErp.Api.Controllers.Finance;
[ApiController,Route("api/budgets"),Authorize]
public class BudgetsController(XeoTechDbContext db):ControllerBase{
[HttpGet]public async Task<IActionResult> Get()=>Ok(await db.Budgets.AsNoTracking().OrderBy(x=>x.Category).ToListAsync());
[HttpPost]public async Task<IActionResult> Upsert(Budget input){if(string.IsNullOrWhiteSpace(input.Category)||input.MonthlyAmount<0)return BadRequest(new{error="Invalid budget."});var b=await db.Budgets.FirstOrDefaultAsync(x=>x.Category==input.Category);if(b is null){input.Id=0;db.Budgets.Add(input);}else b.MonthlyAmount=input.MonthlyAmount;await db.SaveChangesAsync();return Ok(b??input);}
[HttpDelete("{id:int}")]public async Task<IActionResult> Delete(int id){var b=await db.Budgets.FindAsync(id);if(b is null)return NotFound();db.Budgets.Remove(b);await db.SaveChangesAsync();return NoContent();}
}