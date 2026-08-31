using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using Microsoft.EntityFrameworkCore;using XeoTechErp.Api.Data;using XeoTechErp.Api.Models;
namespace XeoTechErp.Api.Controllers.System;
[ApiController,Route("api/settings"),Authorize]
public class SettingsController(XeoTechDbContext db):ControllerBase{
[HttpGet]public async Task<IActionResult> Get()=>Ok(await db.AppConfig.AsNoTracking().FirstOrDefaultAsync()??new AppConfig());
[HttpPut]public async Task<IActionResult> Update(AppConfig input){if(input.TaxRate<0||input.TaxRate>100||input.ShippingFee<0||input.FreeShipOver<0)return BadRequest(new{error="Invalid settings."});var c=await db.AppConfig.FirstOrDefaultAsync();if(c is null){input.Id=1;db.AppConfig.Add(input);c=input;}else{c.TaxRate=input.TaxRate;c.ShippingFee=input.ShippingFee;c.FreeShipOver=input.FreeShipOver;}await db.SaveChangesAsync();return Ok(c);}
}