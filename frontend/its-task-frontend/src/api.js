
const API_URL = "https://localhost:7209/api";

export async function login(email, password) {
  const response = await fetch(`${API_URL}/Auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, password })
  });
  if (!response.ok) throw new Error('Invalid email or password');
  return await response.json();
}

export async function getCurrentUser(token) {
  const response = await fetch(`${API_URL}/Auth/me`, {
    headers: { Authorization: `Bearer ${token}` }
  });
  if (!response.ok) throw new Error('Session is invalid or expired');
  return await response.json();
}

export async function getMyProfile(token) {
  const response = await fetch(`${API_URL}/employee-profile/me`, {
    headers: { Authorization: `Bearer ${token}` }
  });
  if (!response.ok) throw new Error('Failed to load employee profile');
  return await response.json();
}

export async function getMyBalances(token) {
  const response = await fetch(`${API_URL}/LeaveBalances/me`, {
    headers: { Authorization: `Bearer ${token}` }
  });
  if (!response.ok) throw new Error('Failed to load leave balances');
  return await response.json();
}

export async function getLeaveTypes(token) {
  const response = await fetch(`${API_URL}/LeaveTypes`, {
    headers: { Authorization: `Bearer ${token}` }
  });
  if (!response.ok) throw new Error('Failed to load leave types');
  return await response.json();
}

export async function submitLeaveRequest(token, requestData) {
  const response = await fetch(`${API_URL}/LeaveRequests`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`
    },
    body: JSON.stringify(requestData)
  });
  if (!response.ok) {
    throw new Error((await response.text()) || 'Submission failed');
  }
  return await response.json();
}

export async function getMyLeaveRequests(token) {
  const response = await fetch(`${API_URL}/LeaveRequests/my`, {
    headers: { Authorization: `Bearer ${token}` }
  });
  if (!response.ok) throw new Error('Failed to load leave requests');
  return await response.json();
}

export async function getTeamLeaveRequests(token) {
  const response = await fetch(`${API_URL}/LeaveRequests/team`, {
    headers: { Authorization: `Bearer ${token}` }
  });
  if (!response.ok) throw new Error('Failed to load team requests');
  return await response.json();
}

export async function approveLeaveRequest(token, requestId) {
  const response = await fetch(
    `${API_URL}/LeaveRequests/${requestId}/approve`,
    {
      method: 'PUT',
      headers: { Authorization: `Bearer ${token}` }
    }
  );
  if (!response.ok) {
    throw new Error((await response.text()) || 'Approval failed');
  }
  return await response.text();
}

export async function rejectLeaveRequest(token, requestId, reason) {
  const response = await fetch(
    `${API_URL}/LeaveRequests/${requestId}/reject`,
    {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`
      },
      body: JSON.stringify({ rejectionReason: reason })
    }
  );
  if (!response.ok) {
    throw new Error((await response.text()) || 'Rejection failed');
  }
  return await response.text();
}

export async function getEmployees(token) {
  const response = await fetch(`${API_URL}/Employees`, {
    headers: { Authorization: `Bearer ${token}` }
  });
  if (!response.ok) throw new Error('Failed to load employees');
  return await response.json();
}

export async function searchEmployees(token, searchTerm) {
  const response = await fetch(
    `${API_URL}/Employees/search?Search=${encodeURIComponent(searchTerm.trim())}`,
    {
      method: 'GET',
      headers: {
        Authorization: `Bearer ${token}`
      }
    }
  );

  if (!response.ok) {
    const message = await response.text();
    throw new Error(message || 'Failed to search employees');
  }

  return await response.json();
}

export async function deleteEmployee(token, employeeId) {
  const response = await fetch(`${API_URL}/Employees/${employeeId}`, {
    method: 'DELETE',
    headers: { Authorization: `Bearer ${token}` }
  });
  if (!response.ok) {
    throw new Error((await response.text()) || 'Delete failed');
  }
  return true;
}

export async function getDepartments(token) {
  const response = await fetch(`${API_URL}/Departments`, {
    headers: { Authorization: `Bearer ${token}` }
  });
  if (!response.ok) throw new Error('Failed to load departments');
  return await response.json();
}

export async function createEmployee(token, data) {
  const response = await fetch(`${API_URL}/Employees`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`
    },
    body: JSON.stringify(data)
  });
  if (!response.ok) {
    throw new Error((await response.text()) || 'Create failed');
  }
  return await response.text();
}

export async function updateEmployee(token, id, data) {
  const response = await fetch(`${API_URL}/Employees/${id}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`
    },
    body: JSON.stringify(data)
  });
  if (!response.ok) {
    throw new Error((await response.text()) || 'Update failed');
  }
  return await response.text();
}

export async function addDepartment(token, data) {
  const response = await fetch(`${API_URL}/Departments`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`
    },
    body: JSON.stringify(data)
  });
  if (!response.ok) {
    throw new Error((await response.text()) || 'Add department failed');
  }
  return await response.text();
}

export async function addLeaveType(token, data) {
  const response = await fetch(`${API_URL}/LeaveTypes`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`
    },
    body: JSON.stringify(data)
  });
  if (!response.ok) {
    throw new Error((await response.text()) || 'Add leave type failed');
  }
  return await response.text();
}

export async function getAllLeaveBalances(token) {
  const response = await fetch(`${API_URL}/LeaveBalances`, {
    headers: { Authorization: `Bearer ${token}` }
  });

  if (!response.ok) {
    throw new Error('Failed to load all leave balances');
  }

  return await response.json();
}

export async function addLeaveBalance(token, data) {
  const response = await fetch(`${API_URL}/LeaveBalances`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`
    },
    body: JSON.stringify(data)
  });

  if (!response.ok) {
    throw new Error((await response.text()) || 'Add balance failed');
  }

  return await response.text();
}

export async function getAllLeaveRequests(token) {
  const response = await fetch(`${API_URL}/LeaveRequests/all`, {
    method: 'GET',
    headers: {
      Authorization: `Bearer ${token}`
    }
  });

  if (!response.ok) {
    const message = await response.text();
    throw new Error(message || 'Failed to load all leave requests');
  }

  return await response.json();
}


export async function cancelLeaveRequest(token, requestId) {
  const response = await fetch(
    `${API_URL}/LeaveRequests/${requestId}/cancel`,
    {
      method: 'PUT',
      headers: {
        Authorization: `Bearer ${token}`
      }
    }
  );

  if (!response.ok) {
    throw new Error((await response.text()) || 'Cancel failed');
  }

  return await response.text();
}

export async function updateLeaveBalance(token, id, data) {
  const response = await fetch(`${API_URL}/LeaveBalances/${id}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`
    },
    body: JSON.stringify(data)
  });

  if (!response.ok) {
    throw new Error((await response.text()) || 'Update balance failed');
  }

  return await response.text();
}

export async function updateDepartment(token, id, data) {
  const response = await fetch(`${API_URL}/Departments/${id}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`
    },
    body: JSON.stringify(data)
  });

  if (!response.ok) {
    throw new Error((await response.text()) || 'Update department failed');
  }

  return await response.text();
}

export async function deleteDepartment(token, id) {
  const response = await fetch(`${API_URL}/Departments/${id}`, {
    method: 'DELETE',
    headers: {
      Authorization: `Bearer ${token}`
    }
  });

  if (!response.ok) {
    throw new Error((await response.text()) || 'Delete department failed');
  }

  return true;
}

export async function updateLeaveType(token, id, data) {
  const response = await fetch(`${API_URL}/LeaveTypes/${id}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`
    },
    body: JSON.stringify(data)
  });

  if (!response.ok) {
    throw new Error((await response.text()) || 'Update leave type failed');
  }

  return await response.text();
}

export async function deleteLeaveType(token, id) {
  const response = await fetch(`${API_URL}/LeaveTypes/${id}`, {
    method: 'DELETE',
    headers: {
      Authorization: `Bearer ${token}`
    }
  });

  if (!response.ok) {
    throw new Error((await response.text()) || 'Delete leave type failed');
  }

  return true;
}
