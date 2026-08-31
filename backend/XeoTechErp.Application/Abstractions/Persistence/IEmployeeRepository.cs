using XeoTechErp.Domain.Entities;
using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Application.Abstractions.Persistence;

public interface IEmployeeRepository
{
    Task<IReadOnlyList<Employee>> GetAsync(string? department, EmployeeStatus? status, CancellationToken cancellationToken = default);
    Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    void Add(Employee employee);
}
