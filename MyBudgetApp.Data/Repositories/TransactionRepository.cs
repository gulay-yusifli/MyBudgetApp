using Microsoft.EntityFrameworkCore;
using MyBudgetApp.Core.Interfaces;
using MyBudgetApp.Core.Models;

namespace MyBudgetApp.Data.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly BudgetDbContext _context;

    public TransactionRepository(BudgetDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Transaction>> GetAllAsync() =>
        await _context.Transactions
            .Include(t => t.Category)
            .OrderByDescending(t => t.Date)
            .ToListAsync();

    public async Task<Transaction?> GetByIdAsync(int id) =>
        await _context.Transactions
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id);

    public async Task<Transaction> AddAsync(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        await _context.Entry(transaction).Reference(t => t.Category).LoadAsync();
        return transaction;
    }

    public async Task<Transaction> UpdateAsync(Transaction transaction)
    {
        _context.Transactions.Update(transaction);
        await _context.SaveChangesAsync();
        await _context.Entry(transaction).Reference(t => t.Category).LoadAsync();
        return transaction;
    }

    public async Task DeleteAsync(int id)
    {
        var transaction = await _context.Transactions.FindAsync(id);
        if (transaction != null)
        {
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Transaction>> GetFilteredAsync(
        DateTime? startDate, DateTime? endDate, int? categoryId, TransactionType? type)
    {
        var query = _context.Transactions.Include(t => t.Category).AsQueryable();

        if (startDate.HasValue)
            query = query.Where(t => t.Date >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(t => t.Date <= endDate.Value);

        if (categoryId.HasValue)
            query = query.Where(t => t.CategoryId == categoryId.Value);

        if (type.HasValue)
            query = query.Where(t => t.Type == type.Value);

        return await query.OrderByDescending(t => t.Date).ToListAsync();
    }

    public async Task<bool> ExistsAsync(int id) =>
        await _context.Transactions.AnyAsync(t => t.Id == id);
}
