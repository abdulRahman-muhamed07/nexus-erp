using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using Microsoft.EntityFrameworkCore;using XeoTechErp.Api.Data;using XeoTechErp.Api.Models;
namespace XeoTechErp.Api.Controllers.HR;
[ApiController,Route("api/employees"),Authorize]
public class EmployeesController(XeoTechDbContext db):ControllerBase{
[HttpGet]public async Task<IActionResult> Get([FromQuery]string? department,[FromQuery]EmployeeStatus? status){var q=db.Employees.AsNoTracking();if(!string.IsNullOrWhiteSpace(department))q=q.Where(x=>x.Department==department);if(status.HasValue)q=q.Where(x=>x.Status==status);return Ok(await q.OrderBy(x=>x.Name).ToListAsync());}
[HttpGet("{id:int}")]public async Task<IActionResult> GetById(int id)=>Ok(await db.Employees.AsNoTracking().FirstOrDefaultAsync(x=>x.Id==id)??(object)new{error="Employee not found"});
[HttpPost]public async Task<IActionResult> Create(Employee e){if(string.IsNullOrWhiteSpace(e.Name))return BadRequest(new{error="Name is required."});e.Id=0;db.Employees.Add(e);await db.SaveChangesAsync();return CreatedAtAction(nameof(GetById),new{id=e.Id},e);}
[HttpPut("{id:int}")]public async Task<IActionResult> Update(int id,Employee i){var e=await db.Employees.FindAsync(id);if(e is null)return NotFound();e.Name=i.Name;e.JobTitle=i.JobTitle;e.Department=i.Department;e.Email=i.Email;e.Salary=i.Salary;e.Status=i.Status;e.HireDate=i.HireDate;await db.SaveChangesAsync();return Ok(e);}
}