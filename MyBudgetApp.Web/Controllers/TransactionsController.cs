using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyBudgetApp.Core.DTOs;
using MyBudgetApp.Core.Interfaces;
using MyBudgetApp.Core.Models;

namespace MyBudgetApp.Web.Controllers;

public class TransactionsController : Controller
{
    private readonly ITransactionService _transactionService;
    private readonly ICategoryService _categoryService;

    public TransactionsController(ITransactionService transactionService, ICategoryService categoryService)
    {
        _transactionService = transactionService;
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index(TransactionFilterDto? filter)
    {
        filter ??= new TransactionFilterDto();
        var transactions = await _transactionService.GetFilteredAsync(filter);
        var categories = await _categoryService.GetAllAsync();

        ViewBag.Categories = new SelectList(categories, "Id", "Name", filter.CategoryId);
        ViewBag.Filter = filter;
        ViewBag.TotalIncome = await _transactionService.GetTotalIncomeAsync(filter.StartDate, filter.EndDate);
        ViewBag.TotalExpenses = await _transactionService.GetTotalExpensesAsync(filter.StartDate, filter.EndDate);
        ViewBag.Balance = await _transactionService.GetBalanceAsync(filter.StartDate, filter.EndDate);

        return View(transactions);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateCategoriesAsync();
        return View(new Transaction { Date = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Transaction transaction)
    {
        if (!ModelState.IsValid)
        {
            await PopulateCategoriesAsync(transaction.CategoryId);
            return View(transaction);
        }

        try
        {
            await _transactionService.CreateAsync(transaction);
            TempData["Success"] = "Transaction added successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("", ex.Message);
            await PopulateCategoriesAsync(transaction.CategoryId);
            return View(transaction);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var transaction = await _transactionService.GetByIdAsync(id);
        if (transaction == null) return NotFound();

        await PopulateCategoriesAsync(transaction.CategoryId);
        return View(transaction);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Transaction transaction)
    {
        if (id != transaction.Id) return BadRequest();

        if (!ModelState.IsValid)
        {
            await PopulateCategoriesAsync(transaction.CategoryId);
            return View(transaction);
        }

        try
        {
            await _transactionService.UpdateAsync(transaction);
            TempData["Success"] = "Transaction updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("", ex.Message);
            await PopulateCategoriesAsync(transaction.CategoryId);
            return View(transaction);
        }
    }

    public async Task<IActionResult> Delete(int id)
    {
        var transaction = await _transactionService.GetByIdAsync(id);
        if (transaction == null) return NotFound();
        return View(transaction);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _transactionService.DeleteAsync(id);
        TempData["Success"] = "Transaction deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var transaction = await _transactionService.GetByIdAsync(id);
        if (transaction == null) return NotFound();
        return View(transaction);
    }

    private async Task PopulateCategoriesAsync(int? selectedId = null)
    {
        var categories = await _categoryService.GetAllAsync();
        ViewBag.CategoryId = new SelectList(categories, "Id", "Name", selectedId);
    }
}
