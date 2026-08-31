using Microsoft.EntityFrameworkCore;
using XeoTechErp.Api.Data;
using XeoTechErp.Api.DTOs;
using XeoTechErp.Api.Models;

namespace XeoTechErp.Api.Services;
public interface ICustomerService { Task<List<CustomerDto>> GetAsync(string? search); Task<CustomerDto?> GetAsync(int id); Task<CustomerDto> CreateAsync(CreateCustomerRequest r); }
public sealed class CustomerService(XeoTechDbContext db) : ICustomerService
{
    public async Task<List<CustomerDto>> GetAsync(string? search) => await db.Customers.AsNoTracking().Where(c=>string.IsNullOrWhiteSpace(search)||c.Company.Contains(search!)||c.ContactName.Contains(search!)).OrderBy(c=>c.Company).Select(c=>new CustomerDto(c.Id,c.Company,c.ContactName,c.Email,c.Phone,c.Country,c.Tier,c.PaymentTerms,c.CreditLimit,c.OnHold)).ToListAsync();
    public async Task<CustomerDto?> GetAsync(int id)=>await db.Customers.AsNoTracking().Where(c=>c.Id==id).Select(c=>new CustomerDto(c.Id,c.Company,c.ContactName,c.Email,c.Phone,c.Country,c.Tier,c.PaymentTerms,c.CreditLimit,c.OnHold)).SingleOrDefaultAsync();
    public async Task<CustomerDto> CreateAsync(CreateCustomerRequest r){var c=new Customer{Company=r.Company.Trim(),ContactName=r.ContactName?.Trim()??"",Email=r.Email?.Trim()??"",Phone=r.Phone?.Trim()??"",Country=r.Country?.Trim()??"",Tier=r.Tier,PaymentTerms=r.PaymentTerms?.Trim()??"Net 30",CreditLimit=r.CreditLimit};db.Customers.Add(c);await db.SaveChangesAsync();return new CustomerDto(c.Id,c.Company,c.ContactName,c.Email,c.Phone,c.Country,c.Tier,c.PaymentTerms,c.CreditLimit,c.OnHold);}
}
