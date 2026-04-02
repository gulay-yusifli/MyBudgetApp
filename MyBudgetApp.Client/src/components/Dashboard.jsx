import { useEffect, useState } from 'react';
import { getDashboard } from '../services/api';

export default function Dashboard() {
  const [summary, setSummary] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    getDashboard()
      .then(setSummary)
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <p>Loading dashboard...</p>;
  if (error) return <p style={{ color: 'red' }}>Error: {error}</p>;
  if (!summary) return null;

  return (
    <div className="dashboard">
      <h2>Dashboard</h2>
      <div className="summary-cards">
        <div className="card income">
          <h3>Total Income</h3>
          <p>${summary.totalIncome?.toFixed(2)}</p>
        </div>
        <div className="card expenses">
          <h3>Total Expenses</h3>
          <p>${summary.totalExpenses?.toFixed(2)}</p>
        </div>
        <div className="card balance">
          <h3>Balance</h3>
          <p>${summary.balance?.toFixed(2)}</p>
        </div>
        <div className="card transactions">
          <h3>Transactions</h3>
          <p>{summary.transactionCount}</p>
        </div>
      </div>

      {summary.categorySummaries?.length > 0 && (
        <div className="category-summary">
          <h3>By Category</h3>
          <table>
            <thead>
              <tr>
                <th>Category</th>
                <th>Transactions</th>
                <th>Total</th>
              </tr>
            </thead>
            <tbody>
              {summary.categorySummaries.map((c) => (
                <tr key={c.categoryName}>
                  <td>
                    <span
                      style={{
                        display: 'inline-block',
                        width: 12,
                        height: 12,
                        borderRadius: '50%',
                        backgroundColor: c.color,
                        marginRight: 6,
                      }}
                    />
                    {c.categoryName}
                  </td>
                  <td>{c.transactionCount}</td>
                  <td>${c.totalAmount?.toFixed(2)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
