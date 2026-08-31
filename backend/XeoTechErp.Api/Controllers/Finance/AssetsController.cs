using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeoTechErp.Application.Contracts.Finance;
using XeoTechErp.Application.Features.Finance;

namespace XeoTechErp.Api.Controllers.Finance;

[ApiController]
[Route("api/assets")]
[Authorize]
public sealed class AssetsController(IFinanceService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken) => Ok(await service.GetAssetsAsync(cancellationToken));

    [Authorize(Policy = "ManagerOrAdmin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateAssetRequest request, CancellationToken cancellationToken)
        => Created("/api/assets", await service.CreateAssetAsync(request, cancellationToken));

    [Authorize(Policy = "ManagerOrAdmin")]
    [HttpPost("{id:int}/dispose")]
    public async Task<IActionResult> Dispose(int id, CancellationToken cancellationToken) => Ok(await service.DisposeAssetAsync(id, cancellationToken));

    [HttpGet("depreciation")]
    public async Task<IActionResult> Depreciation(CancellationToken cancellationToken) => Ok(await service.GetDepreciationAsync(cancellationToken));
}
