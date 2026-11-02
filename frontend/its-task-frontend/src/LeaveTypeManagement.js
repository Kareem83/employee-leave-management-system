import { useState, useEffect } from 'react';
import {
  getLeaveTypes,
  addLeaveType,
  updateLeaveType,
  deleteLeaveType
} from './api';

function LeaveTypeManagement({ token }) {
  const [types, setTypes] = useState([]);
  const [name, setName] = useState('');
  const [editingId, setEditingId] = useState(null);
  const [error, setError] = useState('');
  const [message, setMessage] = useState('');
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    async function load() {
      try {
        setTypes(await getLeaveTypes(token));
      } catch (err) {
        setError(err.message);
      }
    }

    load();
  }, [token]);

  async function refresh() {
    setTypes(await getLeaveTypes(token));
  }

  function startEdit(type) {
    setEditingId(type.id);
    setName(type.name);
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
        await addLeaveType(token, {
          name: name.trim()
        });
        setMessage('Leave type added successfully');
      } else {
        await updateLeaveType(token, editingId, {
          name: name.trim()
        });
        setMessage('Leave type updated successfully');
      }

      await refresh();
      cancelEdit();
    } catch (err) {
      setError(err.message);
    } finally {
      setSaving(false);
    }
  }

  async function handleDelete(type) {
    if (!window.confirm(`Delete ${type.name}?`)) {
      return;
    }

    setError('');
    setMessage('');

    try {
      await deleteLeaveType(token, type.id);
      await refresh();
      setMessage('Leave type deleted successfully');

      if (editingId === type.id) {
        cancelEdit();
      }
    } catch (err) {
      setError(err.message);
    }
  }

  return (
    <section id="leave-types">
      <h3>Leave Type Management</h3>

      {error && <p className="error-message">{error}</p>}
      {message && <p className="success-message">{message}</p>}

      <table>
        <thead>
          <tr>
            <th>ID</th>
            <th>Leave Type</th>
            <th>Actions</th>
          </tr>
        </thead>

        <tbody>
          {types.map((type) => (
            <tr key={type.id}>
              <td>{type.id}</td>
              <td>{type.name}</td>
              <td>
                <button onClick={() => startEdit(type)}>
                  Edit
                </button>

                <button onClick={() => handleDelete(type)}>
                  Delete
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      <h4>
        {editingId === null
          ? 'Add Leave Type'
          : 'Edit Leave Type'}
      </h4>

      <form onSubmit={handleSave}>
        <input
          value={name}
          onChange={(e) => setName(e.target.value)}
          placeholder="Leave type name"
          required
        />

        <button type="submit" disabled={saving || !name.trim()}>
          {saving
            ? 'Saving...'
            : editingId === null
              ? 'Add Leave Type'
              : 'Update Leave Type'}
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

export default LeaveTypeManagement;