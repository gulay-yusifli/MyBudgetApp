using MyBudgetApp.Core.DTOs;
using MyBudgetApp.Core.Interfaces;
using MyBudgetApp.Core.Models;

namespace MyBudgetApp.Core.Services;

public class DashboardService : IDashboardService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICategoryRepository _categoryRepository;

    public DashboardService(ITransactionRepository transactionRepository, ICategoryRepository categoryRepository)
    {
        _transactionRepository = transactionRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync()
    {
        var transactions = await _transactionRepository.GetAllAsync();
        var transactionList = transactions.ToList();

        var totalIncome = transactionList
            .Where(t => t.Type == TransactionType.Income)
            .Sum(t => t.Amount);

        var totalExpenses = transactionList
            .Where(t => t.Type == TransactionType.Expense)
            .Sum(t => t.Amount);

        return new DashboardSummaryDto
        {
            TotalIncome = totalIncome,
            TotalExpenses = totalExpenses,
            TransactionCount = transactionList.Count,
            CategorySummaries = (await GetCategorySummariesAsync()).ToList(),
            MonthlySummaries = (await GetMonthlySummariesAsync(6)).ToList()
        };
    }

    public async Task<IEnumerable<MonthlySummaryDto>> GetMonthlySummariesAsync(int months = 12)
    {
        var transactions = await _transactionRepository.GetAllAsync();
        var cutoff = DateTime.Today.AddMonths(-months + 1);
        cutoff = new DateTime(cutoff.Year, cutoff.Month, 1);

        var result = transactions
            .Where(t => t.Date >= cutoff)
            .GroupBy(t => new { t.Date.Year, t.Date.Month })
            .Select(g => new MonthlySummaryDto
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                TotalIncome = g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                TotalExpenses = g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount)
            })
            .OrderBy(m => m.Year).ThenBy(m => m.Month)
            .ToList();

        return result;
    }

    public async Task<IEnumerable<CategorySummaryDto>> GetCategorySummariesAsync()
    {
        var transactions = await _transactionRepository.GetAllAsync();

        var result = transactions
            .Where(t => t.Category != null)
            .GroupBy(t => new { t.Category!.Name, t.Category.Color })
            .Select(g => new CategorySummaryDto
            {
                CategoryName = g.Key.Name,
                Color = g.Key.Color,
                TotalAmount = g.Sum(t => t.Amount),
                TransactionCount = g.Count()
            })
            .OrderByDescending(c => c.TotalAmount)
            .ToList();

        return result;
    }
}
