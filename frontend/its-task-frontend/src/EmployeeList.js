
import { useState } from 'react';
import {
  searchEmployees,
  deleteEmployee
} from './api';

function EmployeeList({
  token,
  employees,
  onRefresh,
  onEdit
}) {
  const [searchTerm, setSearchTerm] = useState('');
  const [searchResults, setSearchResults] = useState(null);
  const [error, setError] = useState('');

  function getEmployeeId(employee) {
    return employee.employeeId ?? employee.id;
  }

  async function handleSearch() {
    try {
      setError('');

      if (!searchTerm.trim()) {
        setSearchResults(null);
        return;
      }

      const data = await searchEmployees(
        token,
        searchTerm.trim()
      );

      setSearchResults(data);
    } catch (err) {
      setError(err.message);
    }
  }

  async function handleDelete(employee) {
    const employeeId = getEmployeeId(employee);

    if (employeeId == null) {
      setError('Employee ID is missing');
      return;
    }

    const confirmed = window.confirm(
      `Are you sure you want to delete ${employee.fullName}?`
    );

    if (!confirmed) return;

    try {
      setError('');

      await deleteEmployee(token, employeeId);

      setSearchResults(null);
      setSearchTerm('');

      await onRefresh();
    } catch (err) {
      setError(err.message);
    }
  }

  async function handleRefresh() {
    setSearchTerm('');
    setSearchResults(null);
    setError('');

    await onRefresh();
  }

  const displayedEmployees = searchResults ?? employees;

  return (
    <div>
      <h3>Employees</h3>

      <input
        type="text"
        placeholder="Search name or email"
        value={searchTerm}
        onChange={(e) => setSearchTerm(e.target.value)}
      />

      <button onClick={handleSearch}>
        Search
      </button>

      <button onClick={handleRefresh}>
        Refresh All
      </button>

      {error && <p>{error}</p>}

      {displayedEmployees.length === 0 ? (
        <p>No employees found.</p>
      ) : (
        <table border="1" cellPadding="10">
          <thead>
            <tr>
              <th>ID</th>
              <th>Name</th>
              <th>Email</th>
              <th>Department</th>
              <th>Job Title</th>
              <th>Actions</th>
            </tr>
          </thead>

          <tbody>
            {displayedEmployees.map((employee, index) => {
              const employeeId = getEmployeeId(employee);

              return (
                <tr key={employeeId ?? index}>
                  <td>{employeeId ?? '-'}</td>
                  <td>{employee.fullName}</td>
                  <td>{employee.email}</td>
                  <td>{employee.department}</td>
                  <td>{employee.jobTitle}</td>
                  <td>
                    <button
                      onClick={() => onEdit(employee)}
                    >
                      Edit
                    </button>

                    <button
                      onClick={() => handleDelete(employee)}
                      disabled={employeeId == null}
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      )}
    </div>
  );
}

export default EmployeeList;
