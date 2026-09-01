using AutoMapper;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Application.Common;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Application.Features.Finance.Budgets;

public sealed class BudgetService(
    IFinanceRepository repository,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IBudgetService
{
    public async Task<IReadOnlyList<BudgetResponse>> GetAsync(CancellationToken cancellationToken = default)
        => mapper.Map<IReadOnlyList<BudgetResponse>>(await repository.GetBudgetsAsync(cancellationToken));

    public async Task<Result<BudgetResponse>> UpsertAsync(UpsertBudgetRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Category) || request.MonthlyAmount < 0)
            return Result<BudgetResponse>.Failure("BUDGET_INVALID", "Invalid budget data.");

        var category = request.Category.Trim();
        var budget = await repository.GetBudgetByCategoryAsync(category, cancellationToken);

        if (budget is null)
        {
            budget = new Budget { Category = category, MonthlyAmount = request.MonthlyAmount };
            repository.AddBudget(budget);
        }
        else
        {
            budget.MonthlyAmount = request.MonthlyAmount;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<BudgetResponse>.Success(mapper.Map<BudgetResponse>(budget));
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var budget = await repository.GetBudgetByIdAsync(id, cancellationToken);
        if (budget is null)
            return Result.Failure("BUDGET_NOT_FOUND", "Budget was not found.");

        repository.RemoveBudget(budget);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
