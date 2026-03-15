using MyBudgetApp.Core.DTOs;
using MyBudgetApp.Core.Models;

namespace MyBudgetApp.Core.Interfaces;

public interface ITransactionService
{
    Task<IEnumerable<Transaction>> GetAllAsync();
    Task<Transaction?> GetByIdAsync(int id);
    Task<Transaction> CreateAsync(Transaction transaction);
    Task<Transaction> UpdateAsync(Transaction transaction);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<Transaction>> GetFilteredAsync(TransactionFilterDto filter);
    Task<decimal> GetTotalIncomeAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<decimal> GetTotalExpensesAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<decimal> GetBalanceAsync(DateTime? startDate = null, DateTime? endDate = null);
}
