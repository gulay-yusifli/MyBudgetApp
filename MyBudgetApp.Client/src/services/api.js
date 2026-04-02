const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

async function request(path, options = {}) {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    headers: { 'Content-Type': 'application/json', ...options.headers },
    ...options,
  });

  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || `Request failed: ${response.status}`);
  }

  if (response.status === 204) return null;
  return response.json();
}

// Dashboard
export const getDashboard = () => request('/dashboard');
export const getDashboardMonthly = (months = 12) => request(`/dashboard/monthly?months=${months}`);
export const getDashboardCategories = () => request('/dashboard/categories');

// Transactions
export const getTransactions = (filter = {}) => {
  const params = new URLSearchParams();
  if (filter.startDate) params.append('startDate', filter.startDate);
  if (filter.endDate) params.append('endDate', filter.endDate);
  if (filter.categoryId) params.append('categoryId', filter.categoryId);
  if (filter.type) params.append('type', filter.type);
  const query = params.toString();
  return request(`/transactions${query ? `?${query}` : ''}`);
};
export const getTransaction = (id) => request(`/transactions/${id}`);
export const createTransaction = (data) => request('/transactions', { method: 'POST', body: JSON.stringify(data) });
export const updateTransaction = (id, data) => request(`/transactions/${id}`, { method: 'PUT', body: JSON.stringify(data) });
export const deleteTransaction = (id) => request(`/transactions/${id}`, { method: 'DELETE' });

// Categories
export const getCategories = () => request('/categories');
export const getCategory = (id) => request(`/categories/${id}`);
export const createCategory = (data) => request('/categories', { method: 'POST', body: JSON.stringify(data) });
export const updateCategory = (id, data) => request(`/categories/${id}`, { method: 'PUT', body: JSON.stringify(data) });
export const deleteCategory = (id) => request(`/categories/${id}`, { method: 'DELETE' });
