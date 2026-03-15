using Microsoft.EntityFrameworkCore;
using MyBudgetApp.Core.Interfaces;
using MyBudgetApp.Core.Models;

namespace MyBudgetApp.Data.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly BudgetDbContext _context;

    public CategoryRepository(BudgetDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetAllAsync() =>
        await _context.Categories.OrderBy(c => c.Name).ToListAsync();

    public async Task<Category?> GetByIdAsync(int id) =>
        await _context.Categories.FindAsync(id);

    public async Task<Category> AddAsync(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<Category> UpdateAsync(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category != null)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id) =>
        await _context.Categories.AnyAsync(c => c.Id == id);

    public async Task<bool> HasTransactionsAsync(int id) =>
        await _context.Transactions.AnyAsync(t => t.CategoryId == id);
}
