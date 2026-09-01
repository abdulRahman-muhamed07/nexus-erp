using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeoTechErp.Application.Contracts.Activities;
using XeoTechErp.Application.Services;

namespace XeoTechErp.Api.Controllers.System;

[ApiController]
[Route("api/activities")]
[Authorize]
public sealed class ActivitiesController(IActivityService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ActivityResponse>>> Get(
        [FromQuery] int take = 100,
        CancellationToken cancellationToken = default)
        => Ok(await service.GetRecentAsync(take, cancellationToken));

    [HttpPost]
    [Authorize(Roles = "Manager,Administrator")]
    public async Task<ActionResult<ActivityResponse>> Create(
        CreateActivityRequest request,
        CancellationToken cancellationToken)
    {
        var activity = await service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { }, activity);
    }
}
