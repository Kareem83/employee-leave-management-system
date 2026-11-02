import { useState, useEffect } from 'react';
import './App.css';
import Login from './Login';
import EmployeeDashboard from './EmployeeDashboard';
import ManagerDashboard from './ManagerDashboard';
import AdminDashboard from './AdminDashboard';
import { getCurrentUser } from './api';

function App() {
  const [token, setToken] = useState(
    () => sessionStorage.getItem('token') || ''
  );
  const [user, setUser] = useState(null);
  const [checkingSession, setCheckingSession] = useState(true);

  useEffect(() => {
    async function checkSession() {
      if (!token) {
        setCheckingSession(false);
        return;
      }

      setCheckingSession(true);

      try {
        setUser(await getCurrentUser(token));
      } catch (error) {
        sessionStorage.removeItem('token');
        setToken('');
        setUser(null);
      } finally {
        setCheckingSession(false);
      }
    }

    checkSession();
  }, [token]);

  function handleLogin(newToken) {
    sessionStorage.setItem('token', newToken);
    setToken(newToken);
  }

  function handleLogout() {
    sessionStorage.removeItem('token');
    setToken('');
    setUser(null);
  }

  const navigation = {
    Employee: [
      { title: 'Overview', id: 'overview' },
      { title: 'My Profile', id: 'profile' },
      { title: 'Leave Balances', id: 'balances' },
      { title: 'Request Leave', id: 'request-form' },
      { title: 'My Requests', id: 'requests' }
    ],
    Manager: [
      { title: 'Overview', id: 'overview' },
      { title: 'Team Requests', id: 'team-requests' }
    ],
    Admin: [
      { title: 'Overview', id: 'overview' },
      { title: 'Employees', id: 'employees' },
      { title: 'Departments', id: 'departments' },
      { title: 'Leave Types', id: 'leave-types' },
      { title: 'Leave Balances', id: 'leave-balances' },
      { title: 'Leave Requests', id: 'all-requests' }
    ]
  };

  function goToSection(id) {
    document.getElementById(id)?.scrollIntoView({
      behavior: 'smooth'
    });
  }

  if (checkingSession) {
    return <div className="loading-screen">Checking session...</div>;
  }

  if (!user || !token) {
    return (
      <div className="auth-layout">
        <div className="auth-brand">
          <div className="brand-mark">H</div>
          <h1>HR Workspace</h1>
          <p>Everything your team needs in one place.</p>
        </div>
        <div className="auth-card">
          <Login onLogin={handleLogin} />
        </div>
      </div>
    );
  }

  return (
    <div className="dashboard-layout">
      <aside className="sidebar">
        <div className="sidebar-brand">
          <span className="brand-mark">H</span>
          <div>
            <strong>HR Workspace</strong>
            <small>Management Portal</small>
          </div>
        </div>

        <p className="sidebar-label">WORKSPACE</p>

        <nav>
          {(navigation[user.role] || []).map((item) => (
            <button
              key={item.id}
              className="nav-item"
              onClick={() => goToSection(item.id)}
            >
              {item.title}
            </button>
          ))}
        </nav>

        <div className="sidebar-footer">
          <p>{user.email}</p>
          <span className="role-badge">{user.role}</span>
          <button className="logout-button" onClick={handleLogout}>
            Sign out
          </button>
        </div>
      </aside>

      <main className="main-content" id="overview">
        <header className="topbar">
          <div>
            <p className="eyebrow">WORKSPACE / {user.role}</p>
            <h1>{user.role} Dashboard</h1>
            <p className="muted-text">
              Manage your work from one simple place.
            </p>
          </div>

          <div className="topbar-avatar">
            {user.email?.charAt(0).toUpperCase()}
          </div>
        </header>

        <div className="dashboard-content">
          {user.role === 'Employee' && (
            <EmployeeDashboard
              user={user}
              token={token}
              onLogout={handleLogout}
            />
          )}

          {user.role === 'Manager' && (
            <ManagerDashboard
              user={user}
              token={token}
              onLogout={handleLogout}
            />
          )}

          {user.role === 'Admin' && (
            <AdminDashboard
              user={user}
              token={token}
              onLogout={handleLogout}
            />
          )}
        </div>
      </main>
    </div>
  );
}

export default App;