import { useEffect, useState } from 'react';
import { getCategories, createCategory, updateCategory, deleteCategory } from '../services/api';

const emptyCategory = { id: 0, name: '', color: '#6c757d', description: '' };

export default function Categories() {
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [form, setForm] = useState(null);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    getCategories()
      .then(setCategories)
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, []);

  const handleEdit = (category) => setForm({ ...category });
  const handleNew = () => setForm({ ...emptyCategory });
  const handleCancel = () => setForm(null);

  const handleSave = async () => {
    setSaving(true);
    try {
      if (form.id) {
        await updateCategory(form.id, form);
        setCategories((prev) => prev.map((c) => (c.id === form.id ? { ...form } : c)));
      } else {
        const created = await createCategory(form);
        setCategories((prev) => [...prev, created]);
      }
      setForm(null);
    } catch (err) {
      alert(err.message);
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Delete this category?')) return;
    try {
      await deleteCategory(id);
      setCategories((prev) => prev.filter((c) => c.id !== id));
    } catch (err) {
      alert(err.message);
    }
  };

  if (loading) return <p>Loading categories...</p>;
  if (error) return <p style={{ color: 'red' }}>Error: {error}</p>;

  return (
    <div className="categories">
      <h2>Categories</h2>
      <button onClick={handleNew}>+ New Category</button>

      {form && (
        <div className="category-form">
          <h3>{form.id ? 'Edit Category' : 'New Category'}</h3>
          <label>
            Name:
            <input
              value={form.name}
              onChange={(e) => setForm((f) => ({ ...f, name: e.target.value }))}
            />
          </label>
          <label>
            Color:
            <input
              type="color"
              value={form.color}
              onChange={(e) => setForm((f) => ({ ...f, color: e.target.value }))}
            />
          </label>
          <label>
            Description:
            <input
              value={form.description ?? ''}
              onChange={(e) => setForm((f) => ({ ...f, description: e.target.value }))}
            />
          </label>
          <button onClick={handleSave} disabled={saving}>
            {saving ? 'Saving...' : 'Save'}
          </button>
          <button onClick={handleCancel}>Cancel</button>
        </div>
      )}

      <table>
        <thead>
          <tr>
            <th>Color</th>
            <th>Name</th>
            <th>Description</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {categories.length === 0 ? (
            <tr>
              <td colSpan={4}>No categories found.</td>
            </tr>
          ) : (
            categories.map((c) => (
              <tr key={c.id}>
                <td>
                  <span
                    style={{
                      display: 'inline-block',
                      width: 20,
                      height: 20,
                      borderRadius: '50%',
                      backgroundColor: c.color,
                    }}
                  />
                </td>
                <td>{c.name}</td>
                <td>{c.description}</td>
                <td>
                  <button onClick={() => handleEdit(c)}>Edit</button>
                  <button onClick={() => handleDelete(c.id)}>Delete</button>
                </td>
              </tr>
            ))
          )}
        </tbody>
      </table>
    </div>
  );
}
