
import { useState, useEffect } from 'react';
import { createEmployee, updateEmployee } from './api';

function EmployeeForm({
  token,
  employee,
  employees,
  departments,
  onSaved,
  onCancel
}) {
  const [fullName, setFullName] = useState('');
  const [userName, setUserName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [role, setRole] = useState('Employee');
  const [departmentId, setDepartmentId] = useState('');
  const [jobTitle, setJobTitle] = useState('');
  const [hireDate, setHireDate] = useState('');
  const [managerId, setManagerId] = useState('');

  const [error, setError] = useState('');
  const [saving, setSaving] = useState(false);

  const isEditing = employee != null;

  useEffect(() => {
    setFullName(employee?.fullName || '');
    setUserName(employee?.userName || '');
    setEmail(employee?.email || '');
    setPassword('');
    setRole(employee?.role || 'Employee');
    setDepartmentId(employee?.departmentId ?? '');
    setJobTitle(employee?.jobTitle || '');

    setHireDate(
      employee?.hireDate
        ? employee.hireDate.split('T')[0]
        : ''
    );

    setManagerId(employee?.managerId ?? '');
    setError('');
  }, [employee]);

  async function handleSubmit(e) {
    e.preventDefault();

    setSaving(true);
    setError('');

    try {
      const data = {
        fullName,
        email,
        departmentId: Number(departmentId),
        jobTitle,
        hireDate,
        managerId:
          managerId === '' ? null : Number(managerId)
      };

      if (isEditing) {
        const id = employee.employeeId ?? employee.id;

        if (id == null) {
          throw new Error('Employee ID is missing');
        }

        await updateEmployee(token, id, data);
      } else {
        await createEmployee(token, {
          ...data,
          userName,
          password,
          role
        });
      }

      await onSaved();
    } catch (err) {
      setError(err.message);
    } finally {
      setSaving(false);
    }
  }

  return (
    <div>
      <h3>
        {isEditing ? 'Edit Employee' : 'Add Employee'}
      </h3>

      <form onSubmit={handleSubmit}>
        <div>
          <label>Full Name: </label>
          <input
            type="text"
            value={fullName}
            onChange={(e) => setFullName(e.target.value)}
            required
          />
        </div>

        {!isEditing && (
          <div>
            <div>
              <label>Username: </label>
              <input
                type="text"
                value={userName}
                onChange={(e) => setUserName(e.target.value)}
                required
              />
            </div>

            <div>
              <label>Password: </label>
              <input
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
              />
            </div>

            <div>
              <label>Role: </label>
              <select
                value={role}
                onChange={(e) => setRole(e.target.value)}
              >
                <option value="Employee">Employee</option>
                <option value="Manager">Manager</option>
              </select>
            </div>
          </div>
        )}

        <div>
          <label>Email: </label>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />
        </div>

        <div>
          <label>Department: </label>
          <select
            value={departmentId}
            onChange={(e) => setDepartmentId(e.target.value)}
            required
          >
            <option value="">Select Department</option>

            {departments.map((department) => (
              <option
                key={department.id}
                value={department.id}
              >
                {department.name}
              </option>
            ))}
          </select>
        </div>

        <div>
          <label>Job Title: </label>
          <input
            type="text"
            value={jobTitle}
            onChange={(e) => setJobTitle(e.target.value)}
            required
          />
        </div>

        <div>
          <label>Hire Date: </label>
          <input
            type="date"
            value={hireDate}
            onChange={(e) => setHireDate(e.target.value)}
            required
          />
        </div>

        <div>
          <label>Manager: </label>
          <select
            value={managerId}
            onChange={(e) => setManagerId(e.target.value)}
          >
            <option value="">No Manager</option>

            {employees
              .filter((item) => {
                const id = item.employeeId ?? item.id;
                const currentId =
                  employee?.employeeId ?? employee?.id;

                return id != null && id !== currentId;
              })
              .map((item) => {
                const id = item.employeeId ?? item.id;

                return (
                  <option key={id} value={id}>
                    {item.fullName}
                  </option>
                );
              })}
          </select>
        </div>

        {error && <p>{error}</p>}

        <button
          type="submit"
          disabled={saving || departments.length === 0}
        >
          {saving
            ? 'Saving...'
            : isEditing
              ? 'Update Employee'
              : 'Add Employee'}
        </button>

        <button type="button" onClick={onCancel}>
          Cancel
        </button>
      </form>
    </div>
  );
}

export default EmployeeForm;
