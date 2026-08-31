namespace XeoTechErp.Application.Contracts.Settings;

public sealed record AppConfigResponse(decimal TaxRate, decimal ShippingFee, decimal FreeShipOver);
