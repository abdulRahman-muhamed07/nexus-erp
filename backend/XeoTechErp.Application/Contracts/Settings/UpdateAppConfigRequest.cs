namespace XeoTechErp.Application.Contracts.Settings;

public sealed record UpdateAppConfigRequest(decimal TaxRate, decimal ShippingFee, decimal FreeShipOver);
