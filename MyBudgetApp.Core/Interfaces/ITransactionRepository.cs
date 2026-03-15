using MyBudgetApp.Core.Models;

namespace MyBudgetApp.Core.Interfaces;

public interface ITransactionRepository
{
    Task<IEnumerable<Transaction>> GetAllAsync();
    Task<Transaction?> GetByIdAsync(int id);
    Task<Transaction> AddAsync(Transaction transaction);
    Task<Transaction> UpdateAsync(Transaction transaction);
    Task DeleteAsync(int id);
    Task<IEnumerable<Transaction>> GetFilteredAsync(DateTime? startDate, DateTime? endDate, int? categoryId, TransactionType? type);
    Task<bool> ExistsAsync(int id);
}
