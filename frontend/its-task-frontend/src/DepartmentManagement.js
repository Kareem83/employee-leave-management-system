
import { useState, useEffect } from 'react';
import {
  getDepartments,
  addDepartment,
  updateDepartment,
  deleteDepartment
} from './api';

function DepartmentManagement({ token, onDepartmentsChanged }) {
  const [departments, setDepartments] = useState([]);
  const [name, setName] = useState('');
  const [editingId, setEditingId] = useState(null);
  const [error, setError] = useState('');
  const [message, setMessage] = useState('');
  const [saving, setSaving] = useState(false);
  const [refreshing, setRefreshing] = useState(false);

  useEffect(() => {
    async function load() {
      try {
        setDepartments(await getDepartments(token));
      } catch (err) {
        setError(err.message);
      }
    }

    load();
  }, [token]);

  async function refresh() {
    const data = await getDepartments(token);
    setDepartments(data);

    if (onDepartmentsChanged) {
      await onDepartmentsChanged();
    }
  }

  async function handleRefresh() {
    setRefreshing(true);
    setError('');

    try {
      await refresh();
    } catch (err) {
      setError(err.message);
    } finally {
      setRefreshing(false);
    }
  }

  function startEdit(department) {
    setEditingId(department.id);
    setName(department.name);
    setError('');
    setMessage('');
  }

  function cancelEdit() {
    setEditingId(null);
    setName('');
  }

  async function handleSave(e) {
    e.preventDefault();

    setSaving(true);
    setError('');
    setMessage('');

    try {
      if (editingId === null) {
        await addDepartment(token, {
          name: name.trim()
        });

        setMessage('Department added successfully');
      } else {
        await updateDepartment(token, editingId, {
          name: name.trim()
        });

        setMessage('Department updated successfully');
      }

      await refresh();
      cancelEdit();
    } catch (err) {
      setError(err.message);
    } finally {
      setSaving(false);
    }
  }

  async function handleDelete(department) {
    if (!window.confirm(`Delete ${department.name}?`)) {
      return;
    }

    setError('');
    setMessage('');

    try {
      await deleteDepartment(token, department.id);
      await refresh();

      setMessage('Department deleted successfully');

      if (editingId === department.id) {
        cancelEdit();
      }
    } catch (err) {
      setError(err.message);
    }
  }

  return (
    <section id="departments">
      <div className="section-heading">
        <h3>Department Management</h3>

        <button
          type="button"
          onClick={handleRefresh}
          disabled={refreshing}
        >
          {refreshing ? 'Refreshing...' : 'Refresh Departments'}
        </button>
      </div>

      {error && (
        <p className="error-message">{error}</p>
      )}

      {message && (
        <p className="success-message">{message}</p>
      )}

      <table>
        <thead>
          <tr>
            <th>ID</th>
            <th>Department</th>
            <th>Actions</th>
          </tr>
        </thead>

        <tbody>
          {departments.map((department) => (
            <tr key={department.id}>
              <td>{department.id}</td>
              <td>{department.name}</td>
              <td>
                <button
                  type="button"
                  onClick={() => startEdit(department)}
                >
                  Edit
                </button>

                <button
                  type="button"
                  onClick={() => handleDelete(department)}
                >
                  Delete
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      <h4>
        {editingId === null
          ? 'Add Department'
          : 'Edit Department'}
      </h4>

      <form onSubmit={handleSave}>
        <input
          value={name}
          onChange={(e) => setName(e.target.value)}
          placeholder="Department name"
          required
        />

        <button
          type="submit"
          disabled={saving || !name.trim()}
        >
          {saving
            ? 'Saving...'
            : editingId === null
              ? 'Add Department'
              : 'Update Department'}
        </button>

        {editingId !== null && (
          <button type="button" onClick={cancelEdit}>
            Cancel
          </button>
        )}
      </form>
    </section>
  );
}

export default DepartmentManagement;
