using AutoMapper;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Application.Common;
using XeoTechErp.Application.Contracts.Employees;

namespace XeoTechErp.Application.Services;

public sealed class EmployeeService(IEmployeeRepository repository, IUnitOfWork unitOfWork, IMapper mapper) : IEmployeeService
{
    public async Task<IReadOnlyList<EmployeeResponse>> GetAsync(string? department, XeoTechErp.Domain.Enums.EmployeeStatus? status, CancellationToken cancellationToken = default)
        => mapper.Map<IReadOnlyList<EmployeeResponse>>(await repository.GetAsync(department, status, cancellationToken));

    public async Task<EmployeeResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await repository.GetByIdAsync(id, cancellationToken) is { } employee ? mapper.Map<EmployeeResponse>(employee) : null;

    public async Task<Result<EmployeeResponse>> CreateAsync(CreateEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Email)) return Result<EmployeeResponse>.Failure("EMPLOYEE_INVALID", "Name and email are required.");
        if (request.Salary < 0) return Result<EmployeeResponse>.Failure("EMPLOYEE_INVALID", "Salary cannot be negative.");
        var employee = new Domain.Entities.Employee { Name = request.Name.Trim(), JobTitle = request.JobTitle.Trim(), Department = request.Department.Trim(), Email = request.Email.Trim(), Salary = request.Salary, Status = request.Status, HireDate = request.HireDate == default ? DateTime.UtcNow : request.HireDate };
        repository.Add(employee);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<EmployeeResponse>.Success(mapper.Map<EmployeeResponse>(employee));
    }

    public async Task<Result<EmployeeResponse>> UpdateAsync(int id, UpdateEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        var employee = await repository.GetByIdAsync(id, cancellationToken);
        if (employee is null) return Result<EmployeeResponse>.Failure("EMPLOYEE_NOT_FOUND", "Employee was not found.");
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Email)) return Result<EmployeeResponse>.Failure("EMPLOYEE_INVALID", "Name and email are required.");
        if (request.Salary < 0) return Result<EmployeeResponse>.Failure("EMPLOYEE_INVALID", "Salary cannot be negative.");
        employee.Name = request.Name.Trim(); employee.JobTitle = request.JobTitle.Trim(); employee.Department = request.Department.Trim(); employee.Email = request.Email.Trim(); employee.Salary = request.Salary; employee.Status = request.Status; employee.HireDate = request.HireDate;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<EmployeeResponse>.Success(mapper.Map<EmployeeResponse>(employee));
    }
}
