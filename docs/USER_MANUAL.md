# 📘 MyBudgetApp — User Manual

## Table of Contents

1. [Introduction](#introduction)
2. [Getting Started](#getting-started)
3. [Dashboard](#dashboard)
4. [Transactions](#transactions)
5. [Categories](#categories)
6. [Monthly Reports](#monthly-reports)
7. [Troubleshooting](#troubleshooting)

---

## 1. Introduction

**MyBudgetApp** is a personal finance management web application built with ASP.NET Core 8. It helps you:

- Track income and expense transactions
- Organize spending with custom categories
- Visualize your financial health on a dashboard
- Generate monthly spending reports

---

## 2. Getting Started

### Prerequisites

| Tool | Version | Download |
|------|---------|----------|
| .NET SDK | 8.0+ | https://dotnet.microsoft.com/download |
| SQL Server | 2019+ or LocalDB | https://www.microsoft.com/en-us/sql-server |
| (Optional) Visual Studio | 2022 | https://visualstudio.microsoft.com/ |

### Installation Steps

**Step 1: Clone the repository**
```bash
git clone https://github.com/gulay-yusifli/MyBudgetApp.git
cd MyBudgetApp
```

**Step 2: Configure the database connection string**

Open `MyBudgetApp.Web/appsettings.json` and update the connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MyBudgetApp;Trusted_Connection=True;"
  }
}
```

For a full SQL Server instance:
```json
"DefaultConnection": "Server=YOUR_SERVER;Database=MyBudgetApp;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
```

**Step 3: Restore packages and run**
```bash
dotnet restore
dotnet run --project MyBudgetApp.Web
```

The database is created automatically on first run (migrations are applied at startup).

**Step 4: Open your browser**
Navigate to `https://localhost:5001` or `http://localhost:5000`

---

## 3. Dashboard

The **Dashboard** is your home screen and gives you a quick overview of your finances.

### Summary Cards
- 🟢 **Total Income** — Sum of all income transactions
- 🔴 **Total Expenses** — Sum of all expense transactions
- 🔵 **Balance** — Income minus Expenses (red/yellow if negative)

### Charts
- **Monthly Overview** (bar chart) — Income vs. Expenses for the last 6 months
- **Category Breakdown** (doughnut chart) — Spending distribution by category

### Quick Actions
- Add a new transaction
- View all transactions
- Manage categories
- Browse monthly reports

---

## 4. Transactions

### Viewing Transactions

Navigate to **Transactions** to see a full list. The page shows:
- Date, Category, Description, Type (Income/Expense), Amount

### Filtering Transactions

Use the filter panel to narrow results:
- **Start Date / End Date** — Date range picker
- **Category** — Filter by a specific category
- **Type** — Income only or Expense only

Click **Apply Filter** to update results. Click **Clear** to reset.

### Adding a Transaction

1. Click **"Add Transaction"** button (top right or navbar)
2. Fill in the form:
   - **Amount** — Positive number (e.g., `1500.00`)
   - **Date** — Defaults to today
   - **Type** — Income or Expense
   - **Category** — Select from your categories
   - **Description** — Optional note
3. Click **"Save Transaction"**

### Editing a Transaction

1. In the Transactions list, click the ✏️ edit button
2. Modify any field
3. Click **"Update Transaction"**

### Deleting a Transaction

1. Click the 🗑️ delete button
2. Review the transaction details on the confirmation page
3. Click **"Yes, Delete"** to confirm

---

## 5. Categories

Categories help organize transactions (e.g., Food, Rent, Salary).

### Default Categories

The app comes with 10 pre-loaded categories:
- Salary, Food & Dining, Transport, Rent & Housing
- Healthcare, Entertainment, Education, Shopping, Utilities, Other

### Adding a Category

1. Navigate to **Categories**
2. Click **"Add Category"**
3. Enter a name and pick a color
4. Click **"Save Category"**

### Editing a Category

1. Click the ✏️ edit button on a category card
2. Update the name or color
3. Click **"Update Category"**

### Deleting a Category

> ⚠️ **Note:** Categories that have associated transactions **cannot be deleted**.

1. Click the 🗑️ delete button
2. Confirm deletion

---

## 6. Monthly Reports

Navigate to **Reports** to see a 12-month overview.

### Overview Page

- Line chart showing Income vs. Expenses trends
- Table with monthly totals and net balance
- Click **"View"** on any row for detailed breakdown

### Monthly Detail Page

- Summary cards (Income, Expenses, Balance)
- Full list of transactions for that month
- Category breakdown panel
- Navigation arrows to browse months

---

## 7. Troubleshooting

| Problem | Solution |
|---------|----------|
| Database connection error | Check the connection string in `appsettings.json` |
| Migrations not applied | Run `dotnet ef database update --project MyBudgetApp.Data --startup-project MyBudgetApp.Web` |
| Build error | Run `dotnet restore` then `dotnet build` |
| Tests failing | Ensure `Microsoft.EntityFrameworkCore.InMemory` is installed in MyBudgetApp.Tests |
| Cannot delete category | The category has transactions. Delete those transactions first, or leave the category. |

---

*MyBudgetApp — Course project for .NET development course, 2026*
