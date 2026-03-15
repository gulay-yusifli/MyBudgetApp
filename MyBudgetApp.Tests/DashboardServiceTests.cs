using Moq;
using MyBudgetApp.Core.DTOs;
using MyBudgetApp.Core.Interfaces;
using MyBudgetApp.Core.Models;
using MyBudgetApp.Core.Services;

namespace MyBudgetApp.Tests;

public class DashboardServiceTests
{
    private readonly Mock<ITransactionRepository> _mockTransactionRepo;
    private readonly Mock<ICategoryRepository> _mockCategoryRepo;
    private readonly DashboardService _service;

    public DashboardServiceTests()
    {
        _mockTransactionRepo = new Mock<ITransactionRepository>();
        _mockCategoryRepo = new Mock<ICategoryRepository>();
        _service = new DashboardService(_mockTransactionRepo.Object, _mockCategoryRepo.Object);
    }

    [Fact]
    public async Task GetSummaryAsync_CalculatesCorrectTotals()
    {
        // Arrange
        var transactions = new List<Transaction>
        {
            new Transaction { Amount = 3000, Type = TransactionType.Income, CategoryId = 1, Category = new Category { Name = "Salary", Color = "#28a745" }, Date = DateTime.Today },
            new Transaction { Amount = 500, Type = TransactionType.Income, CategoryId = 1, Category = new Category { Name = "Salary", Color = "#28a745" }, Date = DateTime.Today },
            new Transaction { Amount = 200, Type = TransactionType.Expense, CategoryId = 2, Category = new Category { Name = "Food", Color = "#fd7e14" }, Date = DateTime.Today },
            new Transaction { Amount = 100, Type = TransactionType.Expense, CategoryId = 3, Category = new Category { Name = "Transport", Color = "#17a2b8" }, Date = DateTime.Today }
        };
        _mockTransactionRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(transactions);

        // Act
        var summary = await _service.GetSummaryAsync();

        // Assert
        Assert.Equal(3500, summary.TotalIncome);
        Assert.Equal(300, summary.TotalExpenses);
        Assert.Equal(3200, summary.Balance);
        Assert.Equal(4, summary.TransactionCount);
    }

    [Fact]
    public async Task GetSummaryAsync_EmptyTransactions_ReturnsZeroBalance()
    {
        // Arrange
        _mockTransactionRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Transaction>());

        // Act
        var summary = await _service.GetSummaryAsync();

        // Assert
        Assert.Equal(0, summary.TotalIncome);
        Assert.Equal(0, summary.TotalExpenses);
        Assert.Equal(0, summary.Balance);
        Assert.Equal(0, summary.TransactionCount);
    }

    [Fact]
    public async Task GetMonthlySummariesAsync_GroupsCorrectly()
    {
        // Arrange
        var now = DateTime.Today;
        var transactions = new List<Transaction>
        {
            new Transaction { Amount = 1000, Type = TransactionType.Income, Date = now },
            new Transaction { Amount = 400, Type = TransactionType.Expense, Date = now },
            new Transaction { Amount = 800, Type = TransactionType.Income, Date = now.AddMonths(-1) },
            new Transaction { Amount = 200, Type = TransactionType.Expense, Date = now.AddMonths(-1) }
        };
        _mockTransactionRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(transactions);

        // Act
        var summaries = (await _service.GetMonthlySummariesAsync(12)).ToList();

        // Assert
        Assert.Equal(2, summaries.Count);
        var currentMonth = summaries.FirstOrDefault(s => s.Year == now.Year && s.Month == now.Month);
        Assert.NotNull(currentMonth);
        Assert.Equal(1000, currentMonth.TotalIncome);
        Assert.Equal(400, currentMonth.TotalExpenses);
        Assert.Equal(600, currentMonth.Balance);
    }

    [Fact]
    public async Task GetCategorySummariesAsync_AggregatesByCategory()
    {
        // Arrange
        var category = new Category { Name = "Food", Color = "#fd7e14" };
        var transactions = new List<Transaction>
        {
            new Transaction { Amount = 100, Type = TransactionType.Expense, Category = category, Date = DateTime.Today },
            new Transaction { Amount = 150, Type = TransactionType.Expense, Category = category, Date = DateTime.Today }
        };
        _mockTransactionRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(transactions);

        // Act
        var summaries = (await _service.GetCategorySummariesAsync()).ToList();

        // Assert
        Assert.Single(summaries);
        Assert.Equal("Food", summaries[0].CategoryName);
        Assert.Equal(250, summaries[0].TotalAmount);
        Assert.Equal(2, summaries[0].TransactionCount);
    }

    [Fact]
    public async Task GetMonthlySummariesAsync_ExcludesOldTransactions()
    {
        // Arrange
        var transactions = new List<Transaction>
        {
            new Transaction { Amount = 500, Type = TransactionType.Income, Date = DateTime.Today },
            new Transaction { Amount = 300, Type = TransactionType.Income, Date = DateTime.Today.AddYears(-2) }
        };
        _mockTransactionRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(transactions);

        // Act
        var summaries = (await _service.GetMonthlySummariesAsync(12)).ToList();

        // Assert
        Assert.Single(summaries);
        Assert.Equal(500, summaries[0].TotalIncome);
    }
}
