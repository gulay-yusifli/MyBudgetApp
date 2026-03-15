using Microsoft.AspNetCore.Mvc;
using MyBudgetApp.Core.Interfaces;

namespace MyBudgetApp.Web.Controllers;

public class ReportsController : Controller
{
    private readonly IDashboardService _dashboardService;
    private readonly ITransactionService _transactionService;

    public ReportsController(IDashboardService dashboardService, ITransactionService transactionService)
    {
        _dashboardService = dashboardService;
        _transactionService = transactionService;
    }

    public async Task<IActionResult> Index()
    {
        var monthlySummaries = await _dashboardService.GetMonthlySummariesAsync(12);
        return View(monthlySummaries);
    }

    public async Task<IActionResult> Monthly(int year, int month)
    {
        if (year == 0) year = DateTime.Today.Year;
        if (month == 0) month = DateTime.Today.Month;

        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var transactions = await _transactionService.GetFilteredAsync(new Core.DTOs.TransactionFilterDto
        {
            StartDate = startDate,
            EndDate = endDate
        });

        var categorySummaries = await _dashboardService.GetCategorySummariesAsync();

        ViewBag.Year = year;
        ViewBag.Month = month;
        ViewBag.MonthName = startDate.ToString("MMMM yyyy");
        ViewBag.TotalIncome = await _transactionService.GetTotalIncomeAsync(startDate, endDate);
        ViewBag.TotalExpenses = await _transactionService.GetTotalExpensesAsync(startDate, endDate);
        ViewBag.Balance = await _transactionService.GetBalanceAsync(startDate, endDate);
        ViewBag.CategorySummaries = categorySummaries;

        return View(transactions);
    }
}
