using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Application.Contracts.Employees;

public sealed record EmployeeResponse(int Id, string Name, string JobTitle, string Department, string Email, decimal Salary, EmployeeStatus Status, DateTime HireDate);
