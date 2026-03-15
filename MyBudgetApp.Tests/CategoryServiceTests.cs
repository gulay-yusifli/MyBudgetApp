using Moq;
using MyBudgetApp.Core.Interfaces;
using MyBudgetApp.Core.Models;
using MyBudgetApp.Core.Services;

namespace MyBudgetApp.Tests;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _mockRepo;
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _mockRepo = new Mock<ICategoryRepository>();
        _service = new CategoryService(_mockRepo.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidCategory_ReturnsSavedCategory()
    {
        // Arrange
        var category = new Category { Name = "Food", Color = "#ff5733" };
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Category>()))
            .ReturnsAsync(new Category { Id = 1, Name = "Food", Color = "#ff5733" });

        // Act
        var result = await _service.CreateAsync(category);

        // Assert
        Assert.Equal("Food", result.Name);
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_EmptyName_ThrowsArgumentException()
    {
        // Arrange
        var category = new Category { Name = "", Color = "#ff5733" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(category));
    }

    [Fact]
    public async Task CreateAsync_WhitespaceName_ThrowsArgumentException()
    {
        // Arrange
        var category = new Category { Name = "   ", Color = "#ff5733" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(category));
    }

    [Fact]
    public async Task CreateAsync_NullCategory_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateAsync(null!));
    }

    [Fact]
    public async Task DeleteAsync_CategoryWithTransactions_ThrowsInvalidOperationException()
    {
        // Arrange
        _mockRepo.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _mockRepo.Setup(r => r.HasTransactionsAsync(1)).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.DeleteAsync(1));
    }

    [Fact]
    public async Task DeleteAsync_CategoryWithoutTransactions_ReturnsTrue()
    {
        // Arrange
        _mockRepo.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _mockRepo.Setup(r => r.HasTransactionsAsync(1)).ReturnsAsync(false);
        _mockRepo.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteAsync(1);

        // Assert
        Assert.True(result);
        _mockRepo.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingCategory_ReturnsFalse()
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
    public async Task UpdateAsync_NonExistingCategory_ThrowsKeyNotFoundException()
    {
        // Arrange
        var category = new Category { Id = 99, Name = "Nonexistent" };
        _mockRepo.Setup(r => r.ExistsAsync(99)).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(category));
    }

    [Fact]
    public async Task UpdateAsync_ValidCategory_CallsRepositoryUpdate()
    {
        // Arrange
        var category = new Category { Id = 1, Name = "Updated Name", Color = "#123456" };
        _mockRepo.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Category>())).ReturnsAsync(category);

        // Act
        var result = await _service.UpdateAsync(category);

        // Assert
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Once);
        Assert.Equal("Updated Name", result.Name);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCategories()
    {
        // Arrange
        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Food", Color = "#ff5733" },
            new Category { Id = 2, Name = "Transport", Color = "#17a2b8" }
        };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }
}
