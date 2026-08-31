using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using XeoTechErp.Api.DTOs;
using XeoTechErp.Api.Services;
namespace XeoTechErp.Api.Controllers;
[ApiController,Authorize,Route("api/orders")]
public class OrdersController(IOrderService service):ControllerBase
{
 [HttpGet] public async Task<IActionResult> Get()=>Ok(await service.GetAllAsync());
 [HttpGet("{id:int}")] public async Task<IActionResult> Get(int id)=>(await service.GetAsync(id)) is { } x?Ok(x):NotFound();
 [Authorize(Roles="Manager,Administrator"),HttpPost] public async Task<IActionResult> Create(CreateOrderRequest request){var actor=User.FindFirstValue(ClaimTypes.Email)??User.Identity?.Name??"system";var x=await service.CreateAsync(request,actor);return CreatedAtAction(nameof(Get),new{id=x.Id},x);}
}
