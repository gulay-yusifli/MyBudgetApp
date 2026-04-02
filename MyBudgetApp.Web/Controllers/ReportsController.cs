using Microsoft.AspNetCore.Mvc;
using MyBudgetApp.Core.DTOs;
using MyBudgetApp.Core.Interfaces;

namespace MyBudgetApp.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly ITransactionService _transactionService;

    public ReportsController(IDashboardService dashboardService, ITransactionService transactionService)
    {
        _dashboardService = dashboardService;
        _transactionService = transactionService;
    }

    [HttpGet("monthly")]
    public async Task<IActionResult> GetMonthlySummaries([FromQuery] int months = 12)
    {
        var summaries = await _dashboardService.GetMonthlySummariesAsync(months);
        return Ok(summaries);
    }

    [HttpGet("monthly/{year}/{month}")]
    public async Task<IActionResult> GetMonthlyDetail(int year, int month)
    {
        if (year == 0) year = DateTime.Today.Year;
        if (month == 0) month = DateTime.Today.Month;

        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var transactions = await _transactionService.GetFilteredAsync(new TransactionFilterDto
        {
            StartDate = startDate,
            EndDate = endDate
        });

        var categorySummaries = await _dashboardService.GetCategorySummariesAsync();
        var income = await _transactionService.GetTotalIncomeAsync(startDate, endDate);
        var expenses = await _transactionService.GetTotalExpensesAsync(startDate, endDate);
        var balance = await _transactionService.GetBalanceAsync(startDate, endDate);

        return Ok(new
        {
            year,
            month,
            monthName = startDate.ToString("MMMM yyyy"),
            income,
            expenses,
            balance,
            transactions,
            categorySummaries
        });
    }
}
