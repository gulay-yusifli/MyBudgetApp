# 🏗️ MyBudgetApp — Technical Documentation

## Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Project Structure](#project-structure)
3. [Technology Stack](#technology-stack)
4. [Design Patterns](#design-patterns)
5. [Data Models](#data-models)
6. [Service Layer](#service-layer)
7. [Data Access Layer](#data-access-layer)
8. [Database Schema](#database-schema)
9. [Dependency Injection](#dependency-injection)
10. [Unit Testing Strategy](#unit-testing-strategy)
11. [Running Migrations](#running-migrations)

---

## 1. Architecture Overview

MyBudgetApp follows a **layered (N-tier) architecture** with clear separation of concerns:

```
┌─────────────────────────────────────────────────────┐
│              MyBudgetApp.Web (UI Layer)              │
│         Controllers • Razor Views • wwwroot          │
└──────────────────────┬──────────────────────────────┘
                       │ depends on
┌──────────────────────▼──────────────────────────────┐
│           MyBudgetApp.Core (Business Layer)          │
│    Models • Interfaces • Services • DTOs             │
└────────────────┬────────────────────────────────────┘
                 │ implements interfaces
┌────────────────▼────────────────────────────────────┐
│          MyBudgetApp.Data (Data Access Layer)        │
│    EF Core DbContext • Repositories • Migrations     │
└─────────────────────────────────────────────────────┘
                       │ uses
                ┌──────▼──────┐
                │  SQL Server  │
                └─────────────┘
```

The `MyBudgetApp.Web` project references both `Core` and `Data`, while `Core` has no dependencies on `Data` — it only defines interfaces.

---

## 2. Project Structure

```
MyBudgetApp/
├── MyBudgetApp.Core/
│   ├── Models/
│   │   ├── Transaction.cs
│   │   ├── Category.cs
│   │   └── TransactionType.cs
│   ├── DTOs/
│   │   ├── TransactionFilterDto.cs
│   │   └── DashboardSummaryDto.cs
│   ├── Interfaces/
│   │   ├── ITransactionRepository.cs
│   │   ├── ICategoryRepository.cs
│   │   ├── ITransactionService.cs
│   │   ├── ICategoryService.cs
│   │   └── IDashboardService.cs
│   └── Services/
│       ├── TransactionService.cs
│       ├── CategoryService.cs
│       └── DashboardService.cs
│
├── MyBudgetApp.Data/
│   ├── BudgetDbContext.cs
│   ├── Repositories/
│   │   ├── TransactionRepository.cs
│   │   └── CategoryRepository.cs
│   └── Migrations/
│       └── ...
│
├── MyBudgetApp.Web/
│   ├── Controllers/
│   │   ├── DashboardController.cs
│   │   ├── TransactionsController.cs
│   │   ├── CategoriesController.cs
│   │   └── ReportsController.cs
│   ├── Views/
│   │   ├── Dashboard/Index.cshtml
│   │   ├── Transactions/
│   │   ├── Categories/
│   │   └── Reports/
│   ├── Program.cs
│   └── appsettings.json
│
├── MyBudgetApp.Tests/
│   ├── TransactionServiceTests.cs
│   ├── CategoryServiceTests.cs
│   ├── DashboardServiceTests.cs
│   └── TransactionRepositoryTests.cs
│
└── docs/
    ├── USER_MANUAL.md
    ├── TECHNICAL_DOCS.md
    └── API_DOCS.md
```

---

## 3. Technology Stack

| Layer | Technology | Version |
|-------|-----------|---------|
| Framework | ASP.NET Core MVC | 8.0 |
| Language | C# | 12 |
| ORM | Entity Framework Core | 8.0 |
| Database | SQL Server (MSSQL) | 2019+ / LocalDB |
| Frontend | Bootstrap + Chart.js | 5.3 / 4.4 |
| Icons | Bootstrap Icons | 1.11 |
| Unit Testing | xUnit + Moq | 2.6 / 4.20 |
| In-Memory Testing | EF Core InMemory | 8.0 |

---

## 4. Design Patterns

### Repository Pattern

Each data entity has a dedicated repository interface (`ITransactionRepository`, `ICategoryRepository`) and implementation in the Data layer.

```csharp
// Interface in Core (no EF dependency)
public interface ITransactionRepository
{
    Task<IEnumerable<Transaction>> GetAllAsync();
    Task<Transaction?> GetByIdAsync(int id);
    Task<Transaction> AddAsync(Transaction transaction);
    // ...
}

// Implementation in Data (uses EF Core)
public class TransactionRepository : ITransactionRepository
{
    private readonly BudgetDbContext _context;
    // ...
}
```

### Service Layer (Business Logic)

Services in `MyBudgetApp.Core` contain all business rules and call repository methods.

```csharp
public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _repository;

    public async Task<decimal> GetBalanceAsync(...)
    {
        var income = await GetTotalIncomeAsync(...);
        var expenses = await GetTotalExpensesAsync(...);
        return income - expenses;
    }
}
```

### Dependency Injection

All services and repositories are registered in `Program.cs` using ASP.NET Core's built-in DI container.

---

## 5. Data Models

### Transaction

| Property | Type | Description |
|----------|------|-------------|
| Id | int | Primary key |
| Amount | decimal(18,2) | Positive value |
| Date | DateTime | Transaction date |
| Type | TransactionType | Income or Expense |
| CategoryId | int | FK → Category |
| Description | string? | Optional note (max 500 chars) |

### Category

| Property | Type | Description |
|----------|------|-------------|
| Id | int | Primary key |
| Name | string | Required, max 100 chars |
| Color | string | Hex color code (#RRGGBB) |

### TransactionType (Enum)

```csharp
public enum TransactionType
{
    Income = 1,
    Expense = 2
}
```

---

## 6. Service Layer

### TransactionService

- `GetAllAsync()` — Returns all transactions
- `GetByIdAsync(id)` — Returns single transaction
- `CreateAsync(transaction)` — Validates and saves new transaction
- `UpdateAsync(transaction)` — Validates and updates existing transaction
- `DeleteAsync(id)` — Soft check, then deletes
- `GetFilteredAsync(filter)` — Applies date/category/type filters
- `GetTotalIncomeAsync()` / `GetTotalExpensesAsync()` — Financial totals
- `GetBalanceAsync()` — Net balance calculation

### CategoryService

- `CreateAsync(category)` — Validates name, saves
- `UpdateAsync(category)` — Validates and updates
- `DeleteAsync(id)` — Checks for associated transactions before deleting

### DashboardService

- `GetSummaryAsync()` — All-time summary with charts data
- `GetMonthlySummariesAsync(months)` — Last N months breakdown
- `GetCategorySummariesAsync()` — Per-category totals

---

## 7. Data Access Layer

### BudgetDbContext

Extends `DbContext` with:
- `DbSet<Transaction> Transactions`
- `DbSet<Category> Categories`
- Fluent configuration in `OnModelCreating`
- 10 seeded default categories

### Repositories

Both `TransactionRepository` and `CategoryRepository` use `async/await` throughout. The `TransactionRepository.GetFilteredAsync` builds dynamic LINQ queries based on optional parameters.

---

## 8. Database Schema

```sql
CREATE TABLE Categories (
    Id    INT IDENTITY(1,1) PRIMARY KEY,
    Name  NVARCHAR(100) NOT NULL,
    Color NVARCHAR(7)   NOT NULL DEFAULT '#6c757d'
);

CREATE TABLE Transactions (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    Amount      DECIMAL(18,2) NOT NULL,
    Date        DATETIME2     NOT NULL,
    Type        INT           NOT NULL,  -- 1=Income, 2=Expense
    CategoryId  INT           NOT NULL REFERENCES Categories(Id),
    Description NVARCHAR(500) NULL
);
```

---

## 9. Dependency Injection

Registered in `Program.cs`:

```csharp
// Database
builder.Services.AddDbContext<BudgetDbContext>(options =>
    options.UseSqlServer(connectionString));

// Repositories (Scoped per HTTP request)
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

// Services (Scoped per HTTP request)
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
```

---

## 10. Unit Testing Strategy

### Test Coverage

| Test Class | Tests | Strategy |
|-----------|-------|----------|
| `TransactionServiceTests` | 11 | Moq (mock repository) |
| `CategoryServiceTests` | 9 | Moq (mock repository) |
| `DashboardServiceTests` | 5 | Moq (mock repositories) |
| `TransactionRepositoryTests` | 9 | EF Core InMemory database |

### Test Philosophy

- **Service tests** use `Moq` to mock repositories, testing pure business logic
- **Repository tests** use `EF Core InMemory` provider to test real LINQ queries
- Each test follows **Arrange / Act / Assert** pattern
- Each test is independent (new DB context per repository test)

### Running Tests

```bash
dotnet test
# or with verbose output:
dotnet test --verbosity normal
```

---

## 11. Running Migrations

### Create a new migration

```bash
dotnet ef migrations add <MigrationName> \
  --project MyBudgetApp.Data \
  --startup-project MyBudgetApp.Web
```

### Apply migrations to database

```bash
dotnet ef database update \
  --project MyBudgetApp.Data \
  --startup-project MyBudgetApp.Web
```

> Migrations are also applied automatically at application startup via `db.Database.Migrate()` in `Program.cs`.

---

*MyBudgetApp — Course project for .NET development course, 2026*
