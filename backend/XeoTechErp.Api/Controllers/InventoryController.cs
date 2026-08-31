using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using XeoTechErp.Api.Services;
namespace XeoTechErp.Api.Controllers;
[ApiController,Authorize,Route("api/inventory")]
public class InventoryController(IInventoryService service):ControllerBase
{
 [HttpGet("summary")] public async Task<IActionResult> Summary()=>Ok(await service.GetSummaryAsync());
 [Authorize(Roles="Manager,Administrator"),HttpPost("adjust")] public async Task<IActionResult> Adjust(int productId,int delta,string reason="Manual Adjustment"){var actor=User.FindFirstValue(ClaimTypes.Email)??"system";return await service.AdjustAsync(productId,delta,reason,actor)?Ok():NotFound();}
}
