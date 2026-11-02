
import { useState, useEffect } from 'react';
import {
  getAllLeaveBalances,
  getLeaveTypes,
  addLeaveBalance,
  updateLeaveBalance
} from './api';

function LeaveBalanceManagement({ token, employees }) {
  const [balances, setBalances] = useState([]);
  const [leaveTypes, setLeaveTypes] = useState([]);

  const [editingId, setEditingId] = useState(null);
  const [employeeId, setEmployeeId] = useState('');
  const [leaveTypeId, setLeaveTypeId] = useState('');
  const [year, setYear] = useState(new Date().getFullYear());
  const [allocatedDays, setAllocatedDays] = useState('');

  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [refreshing, setRefreshing] = useState(false);
  const [error, setError] = useState('');
  const [message, setMessage] = useState('');

  useEffect(() => {
    async function loadData() {
      try {
        const [balancesData, typesData] = await Promise.all([
          getAllLeaveBalances(token),
          getLeaveTypes(token)
        ]);

        setBalances(balancesData);
        setLeaveTypes(typesData);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    }

    loadData();
  }, [token]);

  async function refreshBalances() {
    setRefreshing(true);
    setError('');

    try {
      const data = await getAllLeaveBalances(token);
      setBalances(data);
    } catch (err) {
      setError(err.message);
    } finally {
      setRefreshing(false);
    }
  }

  function resetForm() {
    setEditingId(null);
    setEmployeeId('');
    setLeaveTypeId('');
    setYear(new Date().getFullYear());
    setAllocatedDays('');
  }

  function startEdit(balance) {
    setEditingId(balance.id);
    setEmployeeId(balance.employeeId);
    setLeaveTypeId(balance.leaveTypeId);
    setYear(balance.year);
    setAllocatedDays(balance.allocatedDays);
    setError('');
    setMessage('');
  }

  function employeeName(id) {
    const employee = employees.find(
      (item) => (item.employeeId ?? item.id) === id
    );

    return employee ? employee.fullName : id;
  }

  function leaveTypeName(id) {
    const type = leaveTypes.find((item) => item.id === id);
    return type ? type.name : id;
  }

  async function handleSave(e) {
    e.preventDefault();

    setError('');
    setMessage('');

    const days = Number(allocatedDays);

    if (
      allocatedDays === '' ||
      !Number.isInteger(days) ||
      days < 0
    ) {
      setError('Enter a valid number of allocated days');
      return;
    }

    if (editingId !== null) {
      const currentBalance = balances.find(
        (balance) => balance.id === editingId
      );

      if (currentBalance && days < currentBalance.usedDays) {
        setError(
          'Allocated days cannot be less than already used days'
        );
        return;
      }
    }

    setSaving(true);

    try {
      if (editingId === null) {
        await addLeaveBalance(token, {
          employeeId: Number(employeeId),
          leaveTypeId: Number(leaveTypeId),
          year: Number(year),
          allocatedDays: days,
          usedDays: 0
        });

        setMessage('Balance added successfully');
      } else {
        await updateLeaveBalance(token, editingId, {
          allocatedDays: days
        });

        setMessage('Balance updated successfully');
      }

      const updatedBalances = await getAllLeaveBalances(token);
      setBalances(updatedBalances);
      resetForm();
    } catch (err) {
      setError(err.message);
    } finally {
      setSaving(false);
    }
  }

  return (
    <section id="leave-balances">
      <h3>Leave Balance Management</h3>

      <button
        type="button"
        onClick={refreshBalances}
        disabled={refreshing}
      >
        {refreshing ? 'Refreshing...' : 'Refresh Balances'}
      </button>

      {error && (
        <p className="error-message">{error}</p>
      )}

      {message && (
        <p className="success-message">{message}</p>
      )}

      {loading ? (
        <p>Loading balances...</p>
      ) : balances.length === 0 ? (
        <p>No leave balances found.</p>
      ) : (
        <table>
          <thead>
            <tr>
              <th>Employee</th>
              <th>Leave Type</th>
              <th>Year</th>
              <th>Allocated</th>
              <th>Used</th>
              <th>Actions</th>
            </tr>
          </thead>

          <tbody>
            {balances.map((balance) => (
              <tr key={balance.id}>
                <td>{employeeName(balance.employeeId)}</td>
                <td>{leaveTypeName(balance.leaveTypeId)}</td>
                <td>{balance.year}</td>
                <td>{balance.allocatedDays}</td>
                <td>{balance.usedDays}</td>
                <td>
                  <button
                    type="button"
                    onClick={() => startEdit(balance)}
                  >
                    Edit
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      <h4>
        {editingId === null
          ? 'Add Leave Balance'
          : 'Edit Leave Balance'}
      </h4>

      <form onSubmit={handleSave}>
        <div>
          <label>Employee:</label>
          <select
            value={employeeId}
            onChange={(e) => setEmployeeId(e.target.value)}
            disabled={editingId !== null}
            required
          >
            <option value="">Select Employee</option>

            {employees.map((employee) => {
              const id = employee.employeeId ?? employee.id;

              return (
                <option key={id} value={id}>
                  {employee.fullName}
                </option>
              );
            })}
          </select>
        </div>

        <div>
          <label>Leave Type:</label>
          <select
            value={leaveTypeId}
            onChange={(e) => setLeaveTypeId(e.target.value)}
            disabled={editingId !== null}
            required
          >
            <option value="">Select Leave Type</option>

            {leaveTypes.map((type) => (
              <option key={type.id} value={type.id}>
                {type.name}
              </option>
            ))}
          </select>
        </div>

        <div>
          <label>Year:</label>
          <input
            type="number"
            value={year}
            onChange={(e) => setYear(e.target.value)}
            disabled={editingId !== null}
            required
          />
        </div>

        <div>
          <label>Allocated Days:</label>
          <input
            type="number"
            min="0"
            step="1"
            value={allocatedDays}
            onChange={(e) => setAllocatedDays(e.target.value)}
            required
          />
        </div>

        <button type="submit" disabled={saving}>
          {saving
            ? 'Saving...'
            : editingId === null
              ? 'Add Balance'
              : 'Update Balance'}
        </button>

        {editingId !== null && (
          <button
            type="button"
            onClick={resetForm}
          >
            Cancel
          </button>
        )}
      </form>
    </section>
  );
}

export default LeaveBalanceManagement;
