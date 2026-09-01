using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeoTechErp.Application.Services;

namespace XeoTechErp.Api.Controllers.Notifications;

[ApiController]
[Route("api/notifications")]
[Authorize]
public sealed class NotificationsController(INotificationService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] bool unreadOnly = false, [FromQuery] int take = 100, CancellationToken cancellationToken = default)
        => Ok(await service.GetAsync(unreadOnly, take, cancellationToken));

    [HttpPatch("{id:int}/read")]
    public async Task<IActionResult> MarkRead(int id, CancellationToken cancellationToken)
        => await service.MarkReadAsync(id, cancellationToken) ? NoContent() : NotFound();

    [HttpPatch("read-all")]
    public async Task<IActionResult> MarkAllRead(CancellationToken cancellationToken)
    {
        await service.MarkAllReadAsync(cancellationToken);
        return NoContent();
    }
}
