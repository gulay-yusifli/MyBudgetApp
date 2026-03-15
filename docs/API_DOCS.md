# 📡 MyBudgetApp — API Documentation

MyBudgetApp uses **ASP.NET Core MVC** with standard HTTP routes for all operations.

## Base URL

```
http://localhost:5000
```

---

## Dashboard

### GET /Dashboard

Returns the main dashboard with income/expense summaries and charts.

**Response:** HTML page with `DashboardSummaryDto` model:
- `TotalIncome` (decimal)
- `TotalExpenses` (decimal)
- `Balance` (decimal)
- `TransactionCount` (int)
- `CategorySummaries` (list)
- `MonthlySummaries` (list)

---

## Transactions

### GET /Transactions

Returns a list of all transactions. Supports filtering via query parameters.

**Query Parameters:**

| Parameter | Type | Description | Example |
|-----------|------|-------------|---------|
| `StartDate` | date | Filter from date | `2026-01-01` |
| `EndDate` | date | Filter to date | `2026-03-31` |
| `CategoryId` | int | Filter by category | `2` |
| `Type` | string | `Income` or `Expense` | `Expense` |

**Example:**
```
GET /Transactions?StartDate=2026-01-01&EndDate=2026-03-31&Type=Expense
```

---

### GET /Transactions/Create

Returns the form to create a new transaction.

---

### POST /Transactions/Create

Creates a new transaction.

**Form Fields:**

| Field | Required | Validation |
|-------|----------|-----------|
| `Amount` | Yes | > 0 |
| `Date` | Yes | Valid date |
| `Type` | Yes | `Income` or `Expense` |
| `CategoryId` | Yes | Must exist |
| `Description` | No | Max 500 characters |

**Success:** Redirects to `/Transactions` with success message.

**Failure:** Returns form with validation errors.

---

### GET /Transactions/Edit/{id}

Returns the edit form for a specific transaction.

**Parameters:**
- `id` (int) — Transaction ID

**Response:** `404 Not Found` if transaction does not exist.

---

### POST /Transactions/Edit/{id}

Updates an existing transaction.

**Parameters:**
- `id` (int) — Transaction ID in URL, must match form's `Id`

**Form Fields:** Same as Create.

**Success:** Redirects to `/Transactions` with success message.

---

### GET /Transactions/Delete/{id}

Returns the delete confirmation page.

---

### POST /Transactions/Delete/{id}

Deletes a transaction after user confirmation.

**Success:** Redirects to `/Transactions` with success message.

---

### GET /Transactions/Details/{id}

Returns details for a specific transaction.

---

## Categories

### GET /Categories

Returns a list of all categories (card grid layout).

---

### GET /Categories/Create

Returns the create form.

---

### POST /Categories/Create

Creates a new category.

**Form Fields:**

| Field | Required | Validation |
|-------|----------|-----------|
| `Name` | Yes | 1–100 characters |
| `Color` | Yes | Hex code (e.g. `#ff5733`) |

---

### GET /Categories/Edit/{id}

Returns the edit form for a category.

---

### POST /Categories/Edit/{id}

Updates an existing category.

---

### GET /Categories/Delete/{id}

Returns delete confirmation page.

---

### POST /Categories/Delete/{id}

Deletes a category.

**Business Rule:** Categories with associated transactions **cannot be deleted**. Returns an error message via `TempData["Error"]`.

---

## Reports

### GET /Reports

Returns the monthly reports overview with the last 12 months of data.

**Model:** `IEnumerable<MonthlySummaryDto>`

Each item:
- `Year` (int)
- `Month` (int)
- `MonthName` (string) — e.g. "March 2026"
- `TotalIncome` (decimal)
- `TotalExpenses` (decimal)
- `Balance` (decimal)

---

### GET /Reports/Monthly?year={year}&month={month}

Returns a detailed transaction list for a specific month.

**Query Parameters:**

| Parameter | Type | Description |
|-----------|------|-------------|
| `year` | int | Year (defaults to current) |
| `month` | int | Month 1–12 (defaults to current) |

**ViewBag Data:**
- `TotalIncome` — Income for the month
- `TotalExpenses` — Expenses for the month
- `Balance` — Net balance
- `CategorySummaries` — Category breakdown

---

## Error Handling

All controller actions include proper error handling:

- `400 Bad Request` — Returned when URL ID doesn't match model ID
- `404 Not Found` — Returned when entity is not found
- Validation errors — Returned with the form and error messages in `ModelState`
- Business rule violations — Shown via `TempData["Error"]` flash message
- Success confirmations — Shown via `TempData["Success"]` flash message

---

*MyBudgetApp — Course project for .NET development course, 2026*
