using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeoTechErp.Api.Services;
namespace XeoTechErp.Api.Controllers;
[ApiController,Authorize,Route("api/dashboard")]
public class DashboardController(IDashboardService service):ControllerBase { [HttpGet] public async Task<IActionResult> Get()=>Ok(await service.GetAsync()); }
