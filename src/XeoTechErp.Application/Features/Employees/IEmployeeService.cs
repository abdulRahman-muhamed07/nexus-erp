using XeoTechErp.Application.Common;
using XeoTechErp.Application.Contracts.Employees;
using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Application.Services;

public interface IEmployeeService
{
    Task<IReadOnlyList<EmployeeResponse>> GetAsync(string? department, EmployeeStatus? status, CancellationToken cancellationToken = default);
    Task<EmployeeResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<EmployeeResponse>> CreateAsync(CreateEmployeeRequest request, CancellationToken cancellationToken = default);
    Task<Result<EmployeeResponse>> UpdateAsync(int id, UpdateEmployeeRequest request, CancellationToken cancellationToken = default);
}
