using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeoTechErp.Application.Features.Finance;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Api.Controllers.Finance;

[ApiController, Route("api/assets"), Authorize]
public sealed class AssetsController(IFinanceService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken) => Ok(await service.GetAssetsAsync(cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create(Asset asset, CancellationToken cancellationToken)
    {
        var created = await service.CreateAssetAsync(asset, cancellationToken);
        return Created($"/api/assets/{created.Id}", created);
    }

    [HttpPost("{id:int}/dispose")]
    public async Task<IActionResult> Dispose(int id, CancellationToken cancellationToken) => Ok(await service.DisposeAssetAsync(id, cancellationToken));

    [HttpGet("depreciation")]
    public async Task<IActionResult> Depreciation(CancellationToken cancellationToken) => Ok(await service.GetDepreciationAsync(cancellationToken));
}
