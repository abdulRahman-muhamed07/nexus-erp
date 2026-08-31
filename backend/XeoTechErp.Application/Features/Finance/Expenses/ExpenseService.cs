using AutoMapper;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Application.Common;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Application.Features.Finance.Expenses;

public sealed class ExpenseService(
    IFinanceRepository repository,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IExpenseService
{
    public async Task<PagedResult<ExpenseResponse>> GetAsync(int page, int pageSize, string? category, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var data = await repository.GetExpensesAsync(page, pageSize, category, cancellationToken);
        var total = await repository.CountExpensesAsync(category, cancellationToken);
        return new PagedResult<ExpenseResponse>(mapper.Map<IReadOnlyList<ExpenseResponse>>(data), page, pageSize, total);
    }

    public async Task<Result<ExpenseResponse>> CreateAsync(CreateExpenseRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Category) || request.Amount <= 0)
            return Result<ExpenseResponse>.Failure("EXPENSE_INVALID", "Category and a positive amount are required.");

        var expense = new Expense
        {
            Category = request.Category.Trim(),
            Amount = request.Amount,
            Date = request.Date == default ? DateTime.UtcNow : request.Date,
            Description = request.Description?.Trim() ?? string.Empty
        };

        repository.AddExpense(expense);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<ExpenseResponse>.Success(mapper.Map<ExpenseResponse>(expense));
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var expense = await repository.GetExpenseAsync(id, cancellationToken);
        if (expense is null)
            return Result.Failure("EXPENSE_NOT_FOUND", "Expense was not found.");

        repository.RemoveExpense(expense);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
