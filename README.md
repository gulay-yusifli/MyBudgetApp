# 💰 MyBudgetApp

A personal finance management web application built with **ASP.NET Core 8 MVC**, designed to help you track income and expenses, manage budget categories, and analyze spending habits.

---

## 📌 Overview

MyBudgetApp is a practical web application that solves a common everyday problem: **keeping track of where your money goes**. Whether you want to monitor monthly expenses, set budget categories, or simply log your income and spending, MyBudgetApp provides a straightforward and reliable solution.

> This is not a banking app — it's a focused, well-crafted tool that does one thing well: **personal budget management**.

---

## ✨ Features

- ➕ **Add/Edit/Delete Transactions** — Log income and expense entries with amount, date, category, and description
- 📂 **Category Management** — Create and manage custom categories with color coding
- 📊 **Dashboard** — View total income, total expenses, and current balance at a glance with charts
- 🔍 **Filter & Search** — Filter transactions by date range, category, or type
- 📅 **Monthly Reports** — Browse spending summaries broken down by month and category
- 💾 **Persistent Storage** — All data saved in SQL Server via Entity Framework Core
- 🧪 **Unit Tested** — Core business logic (balance calculation, filtering, validation) covered by 39 unit tests

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 MVC |
| Frontend | Bootstrap 5 + Chart.js + Bootstrap Icons |
| Business Logic | C# Service Layer |
| ORM | Entity Framework Core 8 |
| Database | SQL Server (MSSQL) / LocalDB |
| Design Patterns | Repository Pattern, Service Layer, Dependency Injection |
| Unit Testing | xUnit + Moq + EF Core InMemory |
| Version Control | Git + GitHub |
| Language | C# / .NET 8 |

---

## 🗂️ Project Structure

```
MyBudgetApp/
├── MyBudgetApp.Core/         # Business logic, models, interfaces, services
├── MyBudgetApp.Data/         # EF Core DbContext, Repositories, Migrations
├── MyBudgetApp.Web/          # ASP.NET Core MVC Controllers, Views, wwwroot
├── MyBudgetApp.Tests/        # xUnit + Moq unit tests (39 tests)
└── docs/                     # USER_MANUAL.md, TECHNICAL_DOCS.md, API_DOCS.md
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server 2019+](https://www.microsoft.com/en-us/sql-server) or SQL Server LocalDB

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/gulay-yusifli/MyBudgetApp.git
   cd MyBudgetApp
   ```

2. Update the connection string in `MyBudgetApp.Web/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MyBudgetApp;Trusted_Connection=True;"
     }
   }
   ```

3. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

4. Run the application (migrations apply automatically):
   ```bash
   dotnet run --project MyBudgetApp.Web
   ```

5. Open `https://localhost:5001` in your browser.

---

## 🧪 Running Tests

```bash
dotnet test
```

**39 tests** covering:
- Balance calculation logic (`TransactionServiceTests`)
- Transaction validation and filtering
- Category business rules
- Dashboard aggregation logic
- Repository layer with EF Core InMemory

---

## 📖 Documentation

- 📘 [User Manual](docs/USER_MANUAL.md) — Step-by-step guide for end users
- 🏗️ [Technical Documentation](docs/TECHNICAL_DOCS.md) — Architecture, patterns, design decisions
- 📡 [API Documentation](docs/API_DOCS.md) — HTTP routes and parameters

---

## 📄 License

This project is developed as a course assignment for educational purposes.

