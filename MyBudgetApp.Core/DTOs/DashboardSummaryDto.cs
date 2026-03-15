namespace MyBudgetApp.Core.DTOs;

public class DashboardSummaryDto
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal Balance => TotalIncome - TotalExpenses;
    public int TransactionCount { get; set; }
    public List<CategorySummaryDto> CategorySummaries { get; set; } = new();
    public List<MonthlySummaryDto> MonthlySummaries { get; set; } = new();
}

public class CategorySummaryDto
{
    public string CategoryName { get; set; } = string.Empty;
    public string Color { get; set; } = "#6c757d";
    public decimal TotalAmount { get; set; }
    public int TransactionCount { get; set; }
}

public class MonthlySummaryDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName => new DateTime(Year, Month, 1).ToString("MMMM yyyy");
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal Balance => TotalIncome - TotalExpenses;
}
