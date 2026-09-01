using AutoMapper;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Application.Common;
using XeoTechErp.Application.Contracts.Suppliers;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Application.Services;

public sealed class SupplierService(ISupplierRepository repository, IUnitOfWork unitOfWork, IMapper mapper) : ISupplierService
{
    public async Task<IReadOnlyList<SupplierResponse>> GetAsync(string? search, CancellationToken cancellationToken = default)
        => mapper.Map<IReadOnlyList<SupplierResponse>>(await repository.SearchAsync(search, cancellationToken));

    public async Task<SupplierDetailsResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var supplier = await repository.GetByIdAsync(id, cancellationToken);
        return supplier is null ? null : new SupplierDetailsResponse(supplier.Id, supplier.Name, supplier.Contact, supplier.Country, supplier.Rating, supplier.Email, supplier.Phone, supplier.Products.Count, supplier.PurchaseOrders.Count);
    }

    public async Task<Result<SupplierResponse>> CreateAsync(CreateSupplierRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Email))
            return Result<SupplierResponse>.Failure("SUPPLIER_INVALID", "Supplier name and email are required.");
        if (request.Rating is < 0 or > 5)
            return Result<SupplierResponse>.Failure("SUPPLIER_INVALID", "Rating must be between 0 and 5.");

        var supplier = new Supplier { Name = request.Name.Trim(), Contact = request.Contact?.Trim() ?? string.Empty, Country = request.Country?.Trim() ?? string.Empty, Rating = request.Rating, Email = request.Email.Trim(), Phone = request.Phone?.Trim() ?? string.Empty };
        repository.Add(supplier);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<SupplierResponse>.Success(mapper.Map<SupplierResponse>(supplier));
    }

    public async Task<Result<SupplierResponse>> UpdateAsync(int id, UpdateSupplierRequest request, CancellationToken cancellationToken = default)
    {
        var supplier = await repository.GetByIdAsync(id, cancellationToken);
        if (supplier is null) return Result<SupplierResponse>.Failure("SUPPLIER_NOT_FOUND", "Supplier was not found.");
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Email)) return Result<SupplierResponse>.Failure("SUPPLIER_INVALID", "Supplier name and email are required.");
        if (request.Rating is < 0 or > 5) return Result<SupplierResponse>.Failure("SUPPLIER_INVALID", "Rating must be between 0 and 5.");

        supplier.Name = request.Name.Trim(); supplier.Contact = request.Contact?.Trim() ?? string.Empty; supplier.Country = request.Country?.Trim() ?? string.Empty; supplier.Rating = request.Rating; supplier.Email = request.Email.Trim(); supplier.Phone = request.Phone?.Trim() ?? string.Empty;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<SupplierResponse>.Success(mapper.Map<SupplierResponse>(supplier));
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var supplier = await repository.GetByIdAsync(id, cancellationToken);
        if (supplier is null) return Result.Failure("SUPPLIER_NOT_FOUND", "Supplier was not found.");
        if (await repository.HasReferencesAsync(id, cancellationToken)) return Result.Failure("SUPPLIER_IN_USE", "Supplier is referenced by existing records.");
        repository.Remove(supplier);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
