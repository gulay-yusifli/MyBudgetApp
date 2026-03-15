using MyBudgetApp.Core.DTOs;

namespace MyBudgetApp.Core.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync();
    Task<IEnumerable<MonthlySummaryDto>> GetMonthlySummariesAsync(int months = 12);
    Task<IEnumerable<CategorySummaryDto>> GetCategorySummariesAsync();
}
