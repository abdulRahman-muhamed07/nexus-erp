using Microsoft.EntityFrameworkCore;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Domain.Entities;
using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Infrastructure.Persistence.Repositories;

public sealed class EmployeeRepository(XeoTechDbContext db) : IEmployeeRepository
{
    public async Task<IReadOnlyList<Employee>> GetAsync(string? department, EmployeeStatus? status, CancellationToken cancellationToken = default)
    {
        var query = db.Employees.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(department)) query = query.Where(x => x.Department == department);
        if (status.HasValue) query = query.Where(x => x.Status == status);
        return await query.OrderBy(x => x.Name).ToListAsync(cancellationToken);
    }

    public Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default) => db.Employees.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    public void Add(Employee employee) => db.Employees.Add(employee);
}
