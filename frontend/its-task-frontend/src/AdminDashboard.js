
import { useState, useEffect } from 'react';
import {
  getEmployees,
  getDepartments,
  getAllLeaveRequests
} from './api';

import EmployeeList from './EmployeeList';
import EmployeeForm from './EmployeeForm';
import DepartmentManagement from './DepartmentManagement';
import LeaveTypeManagement from './LeaveTypeManagement';
import LeaveBalanceManagement from './LeaveBalanceManagement';
import AdminLeaveRequests from './AdminLeaveRequests';

function AdminDashboard({ user, token, onLogout }) {
  const [employees, setEmployees] = useState([]);
  const [departments, setDepartments] = useState([]);
  const [requests, setRequests] = useState([]);

  const [selectedEmployee, setSelectedEmployee] = useState(null);
  const [showForm, setShowForm] = useState(false);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [message, setMessage] = useState('');

  useEffect(() => {
    async function loadData() {
      try {
        const [employeesData, departmentsData, requestsData] =
          await Promise.all([
            getEmployees(token),
            getDepartments(token),
            getAllLeaveRequests(token)
          ]);

        setEmployees(employeesData);
        setDepartments(departmentsData);
        setRequests(requestsData);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    }

    loadData();
  }, [token]);

  async function refreshEmployees() {
    const data = await getEmployees(token);
    setEmployees(data);
  }

  async function refreshDepartments() {
    const data = await getDepartments(token);
    setDepartments(data);
  }

  async function refreshStatistics() {
    try {
      setError('');

      const [employeesData, requestsData] = await Promise.all([
        getEmployees(token),
        getAllLeaveRequests(token)
      ]);

      setEmployees(employeesData);
      setRequests(requestsData);
    } catch (err) {
      setError(err.message);
    }
  }

  async function handleSaved() {
    await refreshEmployees();
    setMessage('Employee saved successfully');
    setSelectedEmployee(null);
    setShowForm(false);
  }

  const pending = requests.filter(
    (r) => r.status === 'Pending'
  ).length;

  const approved = requests.filter(
    (r) => r.status === 'Approved'
  ).length;

  const rejected = requests.filter(
    (r) => r.status === 'Rejected'
  ).length;

  const cancelled = requests.filter(
    (r) => r.status === 'Cancelled'
  ).length;

  return (
    <div>
      <h2>Admin Dashboard</h2>
      <p>Welcome: {user.email}</p>

      {error && <p className="error-message">{error}</p>}
      {message && <p className="success-message">{message}</p>}

      <section id="statistics">
        <h3>System Statistics</h3>

        {loading ? (
          <p>Loading statistics...</p>
        ) : (
          <div className="stats-grid">
            <div className="stat-card">
              <span>Active Employees</span>
              <strong>{employees.length}</strong>
            </div>

            <div className="stat-card">
              <span>Total Requests</span>
              <strong>{requests.length}</strong>
            </div>

            <div className="stat-card">
              <span>Pending</span>
              <strong>{pending}</strong>
            </div>

            <div className="stat-card">
              <span>Approved</span>
              <strong>{approved}</strong>
            </div>

            <div className="stat-card">
              <span>Rejected</span>
              <strong>{rejected}</strong>
            </div>

            <div className="stat-card">
              <span>Cancelled</span>
              <strong>{cancelled}</strong>
            </div>
          </div>
        )}

        <button onClick={refreshStatistics}>
          Refresh Statistics
        </button>
      </section>

      <hr />

      <section id="employees">
        <h3>Employee Management</h3>

        <button
          onClick={() => {
            setSelectedEmployee(null);
            setShowForm(true);
          }}
        >
          Add Employee
        </button>

        {showForm && (
          <EmployeeForm
            token={token}
            employee={selectedEmployee}
            employees={employees}
            departments={departments}
            onSaved={handleSaved}
            onCancel={() => {
              setSelectedEmployee(null);
              setShowForm(false);
            }}
          />
        )}

        {loading ? (
          <p>Loading employees...</p>
        ) : (
          <EmployeeList
            token={token}
            employees={employees}
            onRefresh={refreshEmployees}
            onEdit={(employee) => {
              setSelectedEmployee(employee);
              setShowForm(true);
            }}
          />
        )}
      </section>

      <hr />

      <DepartmentManagement
        token={token}
        onDepartmentsChanged={refreshDepartments}
      />

      <hr />

      <LeaveTypeManagement token={token} />

      <hr />

      <LeaveBalanceManagement
        token={token}
        employees={employees}
      />

      <hr />

      <AdminLeaveRequests token={token} />

      <hr />

      <button onClick={onLogout}>Logout</button>
    </div>
  );
}

export default AdminDashboard;
