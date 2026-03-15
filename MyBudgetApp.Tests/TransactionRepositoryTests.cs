using Microsoft.EntityFrameworkCore;
using MyBudgetApp.Core.Models;
using MyBudgetApp.Data;
using MyBudgetApp.Data.Repositories;

namespace MyBudgetApp.Tests;

public class TransactionRepositoryTests : IDisposable
{
    private readonly BudgetDbContext _context;
    private readonly TransactionRepository _repository;

    public TransactionRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<BudgetDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new BudgetDbContext(options);
        _repository = new TransactionRepository(_context);

        // Seed a category
        _context.Categories.Add(new Category { Id = 1, Name = "Test Category", Color = "#ff5733" });
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task AddAsync_ValidTransaction_SavesAndReturns()
    {
        // Arrange
        var transaction = new Transaction
        {
            Amount = 500,
            Date = DateTime.Today,
            Type = TransactionType.Income,
            CategoryId = 1,
            Description = "Test income"
        };

        // Act
        var result = await _repository.AddAsync(transaction);

        // Assert
        Assert.True(result.Id > 0);
        Assert.Equal(500, result.Amount);
        Assert.Equal(1, await _context.Transactions.CountAsync());
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsTransaction()
    {
        // Arrange
        var transaction = new Transaction
        {
            Amount = 200,
            Date = DateTime.Today,
            Type = TransactionType.Expense,
            CategoryId = 1
        };
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(transaction.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.Amount);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllTransactions()
    {
        // Arrange
        _context.Transactions.AddRange(
            new Transaction { Amount = 100, Date = DateTime.Today, Type = TransactionType.Income, CategoryId = 1 },
            new Transaction { Amount = 200, Date = DateTime.Today, Type = TransactionType.Expense, CategoryId = 1 },
            new Transaction { Amount = 300, Date = DateTime.Today, Type = TransactionType.Income, CategoryId = 1 }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task DeleteAsync_ExistingTransaction_RemovesIt()
    {
        // Arrange
        var transaction = new Transaction
        {
            Amount = 100,
            Date = DateTime.Today,
            Type = TransactionType.Expense,
            CategoryId = 1
        };
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(transaction.Id);

        // Assert
        Assert.Equal(0, await _context.Transactions.CountAsync());
    }

    [Fact]
    public async Task UpdateAsync_ExistingTransaction_UpdatesValues()
    {
        // Arrange
        var transaction = new Transaction
        {
            Amount = 100,
            Date = DateTime.Today,
            Type = TransactionType.Expense,
            CategoryId = 1,
            Description = "Original"
        };
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        // Act
        transaction.Amount = 250;
        transaction.Description = "Updated";
        var result = await _repository.UpdateAsync(transaction);

        // Assert
        Assert.Equal(250, result.Amount);
        Assert.Equal("Updated", result.Description);
    }

    [Fact]
    public async Task ExistsAsync_ExistingId_ReturnsTrue()
    {
        // Arrange
        var transaction = new Transaction
        {
            Amount = 100, Date = DateTime.Today,
            Type = TransactionType.Income, CategoryId = 1
        };
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsAsync(transaction.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_NonExistingId_ReturnsFalse()
    {
        // Act
        var result = await _repository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetFilteredAsync_ByType_ReturnsOnlyMatchingType()
    {
        // Arrange
        _context.Transactions.AddRange(
            new Transaction { Amount = 100, Date = DateTime.Today, Type = TransactionType.Income, CategoryId = 1 },
            new Transaction { Amount = 200, Date = DateTime.Today, Type = TransactionType.Expense, CategoryId = 1 },
            new Transaction { Amount = 300, Date = DateTime.Today, Type = TransactionType.Income, CategoryId = 1 }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetFilteredAsync(null, null, null, TransactionType.Income);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, t => Assert.Equal(TransactionType.Income, t.Type));
    }

    [Fact]
    public async Task GetFilteredAsync_ByDateRange_ReturnsOnlyMatchingDates()
    {
        // Arrange
        var today = DateTime.Today;
        _context.Transactions.AddRange(
            new Transaction { Amount = 100, Date = today.AddDays(-10), Type = TransactionType.Income, CategoryId = 1 },
            new Transaction { Amount = 200, Date = today, Type = TransactionType.Income, CategoryId = 1 },
            new Transaction { Amount = 300, Date = today.AddDays(10), Type = TransactionType.Income, CategoryId = 1 }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetFilteredAsync(today.AddDays(-5), today.AddDays(5), null, null);

        // Assert
        Assert.Single(result);
        Assert.Equal(200, result.First().Amount);
    }
}
