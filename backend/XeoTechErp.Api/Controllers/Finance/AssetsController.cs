using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeoTechErp.Application.Features.Finance.Assets;

namespace XeoTechErp.Api.Controllers.Finance;

[ApiController]
[Route("api/assets")]
[Authorize]
public sealed class AssetsController(IAssetService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
        => Ok(await service.GetAsync(cancellationToken));

    [Authorize(Policy = "ManagerOrAdmin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateAssetRequest request, CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(request, cancellationToken);
        return result.IsSuccess
            ? Created("/api/assets", result.Value)
            : BadRequest(result.Error);
    }

    [Authorize(Policy = "ManagerOrAdmin")]
    [HttpPost("{id:int}/dispose")]
    public async Task<IActionResult> Dispose(int id, CancellationToken cancellationToken)
    {
        var result = await service.DisposeAsync(id, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpGet("depreciation")]
    public async Task<IActionResult> Depreciation(CancellationToken cancellationToken)
        => Ok(await service.GetDepreciationAsync(cancellationToken));
}
