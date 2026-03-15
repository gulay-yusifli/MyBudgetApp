using MyBudgetApp.Core.DTOs;
using MyBudgetApp.Core.Interfaces;
using MyBudgetApp.Core.Models;

namespace MyBudgetApp.Core.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _repository;

    public TransactionService(ITransactionRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<Transaction>> GetAllAsync() =>
        _repository.GetAllAsync();

    public Task<Transaction?> GetByIdAsync(int id) =>
        _repository.GetByIdAsync(id);

    public async Task<Transaction> CreateAsync(Transaction transaction)
    {
        ArgumentNullException.ThrowIfNull(transaction);

        if (transaction.Amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(transaction));

        if (transaction.Date == default)
            transaction.Date = DateTime.Today;

        return await _repository.AddAsync(transaction);
    }

    public async Task<Transaction> UpdateAsync(Transaction transaction)
    {
        ArgumentNullException.ThrowIfNull(transaction);

        if (transaction.Amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(transaction));

        if (!await _repository.ExistsAsync(transaction.Id))
            throw new KeyNotFoundException($"Transaction with ID {transaction.Id} not found.");

        return await _repository.UpdateAsync(transaction);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (!await _repository.ExistsAsync(id))
            return false;

        await _repository.DeleteAsync(id);
        return true;
    }

    public Task<IEnumerable<Transaction>> GetFilteredAsync(TransactionFilterDto filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        TransactionType? type = null;
        if (!string.IsNullOrWhiteSpace(filter.Type) &&
            Enum.TryParse<TransactionType>(filter.Type, true, out var parsedType))
        {
            type = parsedType;
        }

        return _repository.GetFilteredAsync(filter.StartDate, filter.EndDate, filter.CategoryId, type);
    }

    public async Task<decimal> GetTotalIncomeAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var transactions = await _repository.GetFilteredAsync(startDate, endDate, null, TransactionType.Income);
        return transactions.Sum(t => t.Amount);
    }

    public async Task<decimal> GetTotalExpensesAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var transactions = await _repository.GetFilteredAsync(startDate, endDate, null, TransactionType.Expense);
        return transactions.Sum(t => t.Amount);
    }

    public async Task<decimal> GetBalanceAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var income = await GetTotalIncomeAsync(startDate, endDate);
        var expenses = await GetTotalExpensesAsync(startDate, endDate);
        return income - expenses;
    }
}
