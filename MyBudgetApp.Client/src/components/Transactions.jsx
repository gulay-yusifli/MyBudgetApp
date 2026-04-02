import { useEffect, useState, useCallback } from 'react';
import { getTransactions, deleteTransaction, getCategories } from '../services/api';

export default function Transactions() {
  const [transactions, setTransactions] = useState([]);
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [filter, setFilter] = useState({ startDate: '', endDate: '', categoryId: '', type: '' });

  const loadTransactions = useCallback(() => {
    setLoading(true);
    const activeFilter = {};
    if (filter.startDate) activeFilter.startDate = filter.startDate;
    if (filter.endDate) activeFilter.endDate = filter.endDate;
    if (filter.categoryId) activeFilter.categoryId = filter.categoryId;
    if (filter.type) activeFilter.type = filter.type;

    getTransactions(activeFilter)
      .then(setTransactions)
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, [filter]);

  useEffect(() => {
    getCategories().then(setCategories).catch(() => {});
  }, []);

  useEffect(() => {
    loadTransactions();
  }, [loadTransactions]);

  const handleDelete = async (id) => {
    if (!window.confirm('Delete this transaction?')) return;
    try {
      await deleteTransaction(id);
      setTransactions((prev) => prev.filter((t) => t.id !== id));
    } catch (err) {
      alert(err.message);
    }
  };

  return (
    <div className="transactions">
      <h2>Transactions</h2>

      <div className="filters">
        <input
          type="date"
          value={filter.startDate}
          onChange={(e) => setFilter((f) => ({ ...f, startDate: e.target.value }))}
          placeholder="Start date"
        />
        <input
          type="date"
          value={filter.endDate}
          onChange={(e) => setFilter((f) => ({ ...f, endDate: e.target.value }))}
          placeholder="End date"
        />
        <select
          value={filter.categoryId}
          onChange={(e) => setFilter((f) => ({ ...f, categoryId: e.target.value }))}
        >
          <option value="">All Categories</option>
          {categories.map((c) => (
            <option key={c.id} value={c.id}>
              {c.name}
            </option>
          ))}
        </select>
        <select
          value={filter.type}
          onChange={(e) => setFilter((f) => ({ ...f, type: e.target.value }))}
        >
          <option value="">All Types</option>
          <option value="Income">Income</option>
          <option value="Expense">Expense</option>
        </select>
        <button onClick={loadTransactions}>Filter</button>
      </div>

      {loading && <p>Loading...</p>}
      {error && <p style={{ color: 'red' }}>Error: {error}</p>}

      {!loading && !error && (
        <table>
          <thead>
            <tr>
              <th>Date</th>
              <th>Type</th>
              <th>Category</th>
              <th>Description</th>
              <th>Amount</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {transactions.length === 0 ? (
              <tr>
                <td colSpan={6}>No transactions found.</td>
              </tr>
            ) : (
              transactions.map((t) => (
                <tr key={t.id}>
                  <td>{new Date(t.date).toLocaleDateString()}</td>
                  <td>{t.type}</td>
                  <td>{t.category?.name ?? t.categoryId}</td>
                  <td>{t.description}</td>
                  <td style={{ color: t.type === 'Income' ? 'green' : 'red' }}>
                    ${t.amount?.toFixed(2)}
                  </td>
                  <td>
                    <button onClick={() => handleDelete(t.id)}>Delete</button>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      )}
    </div>
  );
}
