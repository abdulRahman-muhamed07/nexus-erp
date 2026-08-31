using AutoMapper;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Application.Common;
using XeoTechErp.Application.Contracts.Settings;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Application.Services;

public sealed class SettingsService(IAppConfigRepository repository, IUnitOfWork unitOfWork, IMapper mapper) : ISettingsService
{
    public async Task<AppConfigResponse> GetAsync(CancellationToken cancellationToken = default)
    {
        var config = await repository.GetAsync(cancellationToken) ?? new AppConfig();
        return mapper.Map<AppConfigResponse>(config);
    }

    public async Task<Result<AppConfigResponse>> UpdateAsync(UpdateAppConfigRequest request, CancellationToken cancellationToken = default)
    {
        if (request.TaxRate is < 0 or > 100 || request.ShippingFee < 0 || request.FreeShipOver < 0)
            return Result<AppConfigResponse>.Failure("SETTINGS_INVALID", "Tax rate must be 0-100 and monetary settings cannot be negative.");

        var config = await repository.GetAsync(cancellationToken);
        if (config is null)
        {
            config = new AppConfig { Id = 1, TaxRate = request.TaxRate, ShippingFee = request.ShippingFee, FreeShipOver = request.FreeShipOver };
            repository.Add(config);
        }
        else
        {
            config.TaxRate = request.TaxRate;
            config.ShippingFee = request.ShippingFee;
            config.FreeShipOver = request.FreeShipOver;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<AppConfigResponse>.Success(mapper.Map<AppConfigResponse>(config));
    }
}
