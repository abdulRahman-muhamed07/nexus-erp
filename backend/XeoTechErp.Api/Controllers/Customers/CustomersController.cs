using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeoTechErp.Api.DTOs;
using XeoTechErp.Api.Services;
namespace XeoTechErp.Api.Controllers;
[ApiController,Authorize,Route("api/customers")]
public class CustomersController(ICustomerService service):ControllerBase
{
 [HttpGet] public async Task<IActionResult> Get([FromQuery]string? search)=>Ok(await service.GetAsync(search));
 [HttpGet("{id:int}")] public async Task<IActionResult> Get(int id)=>(await service.GetAsync(id)) is { } x?Ok(x):NotFound();
 [Authorize(Roles="Manager,Administrator"),HttpPost] public async Task<IActionResult> Create(CreateCustomerRequest request){var x=await service.CreateAsync(request);return CreatedAtAction(nameof(Get),new{id=x.Id},x);}
}
