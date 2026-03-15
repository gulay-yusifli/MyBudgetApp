using Moq;
using MyBudgetApp.Core.DTOs;
using MyBudgetApp.Core.Interfaces;
using MyBudgetApp.Core.Models;
using MyBudgetApp.Core.Services;

namespace MyBudgetApp.Tests;

public class TransactionServiceTests
{
    private readonly Mock<ITransactionRepository> _mockRepo;
    private readonly TransactionService _service;

    public TransactionServiceTests()
    {
        _mockRepo = new Mock<ITransactionRepository>();
        _service = new TransactionService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetBalanceAsync_WithIncomeAndExpense_ReturnsCorrectBalance()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetFilteredAsync(null, null, null, TransactionType.Income))
            .ReturnsAsync(new List<Transaction>
            {
                new Transaction { Amount = 1000, Type = TransactionType.Income },
                new Transaction { Amount = 500, Type = TransactionType.Income }
            });
        _mockRepo.Setup(r => r.GetFilteredAsync(null, null, null, TransactionType.Expense))
            .ReturnsAsync(new List<Transaction>
            {
                new Transaction { Amount = 300, Type = TransactionType.Expense }
            });

        // Act
        var balance = await _service.GetBalanceAsync();

        // Assert
        Assert.Equal(1200, balance);
    }

    [Fact]
    public async Task GetTotalIncomeAsync_ReturnsCorrectSum()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetFilteredAsync(null, null, null, TransactionType.Income))
            .ReturnsAsync(new List<Transaction>
            {
                new Transaction { Amount = 2000, Type = TransactionType.Income },
                new Transaction { Amount = 1500, Type = TransactionType.Income }
            });

        // Act
        var income = await _service.GetTotalIncomeAsync();

        // Assert
        Assert.Equal(3500, income);
    }

    [Fact]
    public async Task GetTotalExpensesAsync_ReturnsCorrectSum()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetFilteredAsync(null, null, null, TransactionType.Expense))
            .ReturnsAsync(new List<Transaction>
            {
                new Transaction { Amount = 100, Type = TransactionType.Expense },
                new Transaction { Amount = 250, Type = TransactionType.Expense }
            });

        // Act
        var expenses = await _service.GetTotalExpensesAsync();

        // Assert
        Assert.Equal(350, expenses);
    }

    [Fact]
    public async Task CreateAsync_ValidTransaction_CallsRepositoryAdd()
    {
        // Arrange
        var transaction = new Transaction
        {
            Amount = 500,
            Date = DateTime.Today,
            Type = TransactionType.Income,
            CategoryId = 1
        };
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Transaction>()))
            .ReturnsAsync(transaction);

        // Act
        var result = await _service.CreateAsync(transaction);

        // Assert
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Transaction>()), Times.Once);
        Assert.Equal(500, result.Amount);
    }

    [Fact]
    public async Task CreateAsync_ZeroAmount_ThrowsArgumentException()
    {
        // Arrange
        var transaction = new Transaction
        {
            Amount = 0,
            Type = TransactionType.Expense,
            CategoryId = 1
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(transaction));
    }

    [Fact]
    public async Task CreateAsync_NegativeAmount_ThrowsArgumentException()
    {
        // Arrange
        var transaction = new Transaction
        {
            Amount = -100,
            Type = TransactionType.Expense,
            CategoryId = 1
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(transaction));
    }

    [Fact]
    public async Task CreateAsync_NullTransaction_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateAsync(null!));
    }

    [Fact]
    public async Task DeleteAsync_ExistingTransaction_ReturnsTrue()
    {
        // Arrange
        _mockRepo.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _mockRepo.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteAsync(1);

        // Assert
        Assert.True(result);
        _mockRepo.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingTransaction_ReturnsFalse()
    {
        // Arrange
        _mockRepo.Setup(r => r.ExistsAsync(99)).ReturnsAsync(false);

        // Act
        var result = await _service.DeleteAsync(99);

        // Assert
        Assert.False(result);
        _mockRepo.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingTransaction_ThrowsKeyNotFoundException()
    {
        // Arrange
        var transaction = new Transaction { Id = 99, Amount = 100, CategoryId = 1 };
        _mockRepo.Setup(r => r.ExistsAsync(99)).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(transaction));
    }

    [Fact]
    public async Task GetFilteredAsync_WithTypeFilter_PassesCorrectType()
    {
        // Arrange
        var filter = new TransactionFilterDto { Type = "Income" };
        var expected = new List<Transaction>
        {
            new Transaction { Amount = 1000, Type = TransactionType.Income }
        };
        _mockRepo.Setup(r => r.GetFilteredAsync(null, null, null, TransactionType.Income))
            .ReturnsAsync(expected);

        // Act
        var result = await _service.GetFilteredAsync(filter);

        // Assert
        Assert.Single(result);
        _mockRepo.Verify(r => r.GetFilteredAsync(null, null, null, TransactionType.Income), Times.Once);
    }

    [Fact]
    public async Task GetFilteredAsync_WithInvalidType_PassesNullType()
    {
        // Arrange
        var filter = new TransactionFilterDto { Type = "InvalidType" };
        _mockRepo.Setup(r => r.GetFilteredAsync(null, null, null, null))
            .ReturnsAsync(new List<Transaction>());

        // Act
        var result = await _service.GetFilteredAsync(filter);

        // Assert
        _mockRepo.Verify(r => r.GetFilteredAsync(null, null, null, null), Times.Once);
    }

    [Fact]
    public async Task GetBalanceAsync_WithNegativeBalance_ReturnsNegativeValue()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetFilteredAsync(null, null, null, TransactionType.Income))
            .ReturnsAsync(new List<Transaction>
            {
                new Transaction { Amount = 200, Type = TransactionType.Income }
            });
        _mockRepo.Setup(r => r.GetFilteredAsync(null, null, null, TransactionType.Expense))
            .ReturnsAsync(new List<Transaction>
            {
                new Transaction { Amount = 500, Type = TransactionType.Expense }
            });

        // Act
        var balance = await _service.GetBalanceAsync();

        // Assert
        Assert.Equal(-300, balance);
    }

    [Fact]
    public async Task GetAllAsync_CallsRepository()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Transaction>());

        // Act
        await _service.GetAllAsync();

        // Assert
        _mockRepo.Verify(r => r.GetAllAsync(), Times.Once);
    }
}
