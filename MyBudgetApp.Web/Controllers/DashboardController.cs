using Microsoft.AspNetCore.Mvc;
using MyBudgetApp.Core.Interfaces;

namespace MyBudgetApp.Web.Controllers;

public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index()
    {
        var summary = await _dashboardService.GetSummaryAsync();
        return View(summary);
    }
}
