using XeoTechErp.Application.Common;
using XeoTechErp.Application.Contracts.Settings;

namespace XeoTechErp.Application.Services;

public interface ISettingsService
{
    Task<AppConfigResponse> GetAsync(CancellationToken cancellationToken = default);
    Task<Result<AppConfigResponse>> UpdateAsync(UpdateAppConfigRequest request, CancellationToken cancellationToken = default);
}
