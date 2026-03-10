# MyBudgetApp
# 💰 MyBudgetApp

A desktop personal finance management application built with **.NET / WPF**, designed to help users track their income and expenses, manage budget categories, and gain insight into their spending habits — all from a clean and intuitive interface.

---

## 📌 Overview

MyBudgetApp is a practical desktop application that solves a common everyday problem: **keeping track of where your money goes**. Whether you want to monitor monthly expenses, set budget limits per category, or simply log your income and spending, MyBudgetApp provides a straightforward and reliable solution.

> This is not a banking app or a fintech startup product — it is a focused, well-crafted tool that does one thing and does it well: **personal budget management**.

---

## ✨ Features

- ➕ **Add Transactions** — Log income and expense entries with amount, date, category, and description
- 📂 **Category Management** — Create and manage custom categories (e.g., Food, Rent, Salary, Transport)
- 📊 **Dashboard & Summary** — View total income, total expenses, and current balance at a glance
- 🔍 **Filter & Search** — Filter transactions by date range, category, or type (income / expense)
- 📅 **Monthly Reports** — Browse spending summaries broken down by month and category
- 🗑️ **Edit & Delete** — Update or remove any existing transaction
- 💾 **Persistent Storage** — All data is saved locally using a SQLite database via Entity Framework Core
- 🧪 **Unit Tested** — Core business logic (balance calculation, filtering, validation) is covered by unit tests

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| UI | WPF (Windows Presentation Foundation) |
| Architecture Pattern | MVVM (Model-View-ViewModel) |
| Data Access | Entity Framework Core |
| Database | SQLite (local, no server required) |
| Design Patterns | Repository Pattern, Dependency Injection |
| Unit Testing | xUnit + Moq |
| Version Control | Git + GitHub |
| Language | C# / .NET 8 |

---

## 🗂️ Project Structure

```
MyBudgetApp/
├── MyBudgetApp.UI/          # WPF Views and ViewModels
├── MyBudgetApp.Core/        # Business logic, models, interfaces
├── MyBudgetApp.Data/        # EF Core DbContext, Repositories, Migrations
├── MyBudgetApp.Tests/       # Unit tests (xUnit)
└── docs/                    # User manual and technical documentation
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (or any IDE supporting WPF)
- Windows OS (required for WPF)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/your-username/MyBudgetApp.git
   cd MyBudgetApp
   ```

2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

3. Apply database migrations:
   ```bash
   dotnet ef database update --project MyBudgetApp.Data
   ```

4. Run the application:
   ```bash
   dotnet run --project MyBudgetApp.UI
   ```

---

## 🧪 Running Tests

```bash
dotnet test MyBudgetApp.Tests
```

Tests cover:
- Balance calculation logic
- Transaction filtering and validation
- Repository layer (using in-memory database)

---

## 📖 Documentation

- 📘 [User Manual](docs/USER_MANUAL.md) — Step-by-step guide for end users
- 🏗️ [Technical Documentation](docs/TECHNICAL_DOCS.md) — Architecture, patterns, and design decisions

---



## 📄 License

This project is developed as a course assignment for educational purposes.
