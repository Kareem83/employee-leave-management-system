import { useState, useEffect } from 'react';
import {
  getTeamLeaveRequests,
  approveLeaveRequest,
  rejectLeaveRequest
} from './api';

function ManagerDashboard({ user, token, onLogout }) {
  const [requests, setRequests] = useState([]);
  const [filter, setFilter] = useState('All');
  const [rejectionReasons, setRejectionReasons] = useState({});
  const [loading, setLoading] = useState(true);
  const [processingId, setProcessingId] = useState(null);
  const [error, setError] = useState('');
  const [message, setMessage] = useState('');

  useEffect(() => {
    async function loadInitialRequests() {
      try {
        setRequests(await getTeamLeaveRequests(token));
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    }

    loadInitialRequests();
  }, [token]);

  async function refreshRequests() {
    try {
      setError('');
      setRequests(await getTeamLeaveRequests(token));
    } catch (err) {
      setError(err.message);
    }
  }

  async function handleApprove(id) {
    setProcessingId(id);
    setError('');
    setMessage('');

    try {
      await approveLeaveRequest(token, id);
      await refreshRequests();
      setMessage('Leave request approved successfully');
    } catch (err) {
      setError(err.message);
    } finally {
      setProcessingId(null);
    }
  }

  async function handleReject(id) {
    const reason = (rejectionReasons[id] || '').trim();

    if (reason.length < 3) {
      setError('Rejection reason must be at least 3 characters');
      return;
    }

    setProcessingId(id);
    setError('');
    setMessage('');

    try {
      await rejectLeaveRequest(token, id, reason);
      await refreshRequests();
      setMessage('Leave request rejected successfully');
    } catch (err) {
      setError(err.message);
    } finally {
      setProcessingId(null);
    }
  }

  function formatDate(value) {
    return value ? value.split('T')[0] : '-';
  }

  const totalRequests = requests.length;
  const pendingRequests = requests.filter(
    (r) => r.status === 'Pending'
  ).length;
  const approvedRequests = requests.filter(
    (r) => r.status === 'Approved'
  ).length;
  const rejectedRequests = requests.filter(
    (r) => r.status === 'Rejected'
  ).length;

  const filteredRequests = requests.filter(
    (r) => filter === 'All' || r.status === filter
  );

  return (
    <div>
      <h2>Manager Dashboard</h2>
      <p>Welcome: {user.email}</p>

      <section id="statistics">
        <h3>Team Statistics</h3>

        {loading ? (
          <p>Loading statistics...</p>
        ) : (
          <div className="stats-grid">
            <div className="stat-card">
              <span>Total Requests</span>
              <strong>{totalRequests}</strong>
            </div>
            <div className="stat-card">
              <span>Pending</span>
              <strong>{pendingRequests}</strong>
            </div>
            <div className="stat-card">
              <span>Approved</span>
              <strong>{approvedRequests}</strong>
            </div>
            <div className="stat-card">
              <span>Rejected</span>
              <strong>{rejectedRequests}</strong>
            </div>
          </div>
        )}
      </section>

      <section id="team-requests">
        <h3>Team Leave Requests</h3>

        <label>Filter: </label>
        <select
          value={filter}
          onChange={(e) => setFilter(e.target.value)}
        >
          <option value="All">All</option>
          <option value="Pending">Pending</option>
          <option value="Approved">Approved</option>
          <option value="Rejected">Rejected</option>
          <option value="Cancelled">Cancelled</option>
        </select>

        <button onClick={refreshRequests}>
          Refresh
        </button>

        {error && <p className="error-message">{error}</p>}
        {message && <p className="success-message">{message}</p>}

        {loading ? (
          <p>Loading team requests...</p>
        ) : filteredRequests.length === 0 ? (
          <p>No leave requests found.</p>
        ) : (
          <table>
            <thead>
              <tr>
                <th>Employee</th>
                <th>Leave Type</th>
                <th>Start</th>
                <th>End</th>
                <th>Days</th>
                <th>Reason</th>
                <th>Status</th>
                <th>Reviewed By</th>
                <th>Actions</th>
              </tr>
            </thead>

            <tbody>
              {filteredRequests.map((request) => (
                <tr key={request.id}>
                  <td>{request.employeeName}</td>
                  <td>{request.leaveTypeName}</td>
                  <td>{formatDate(request.startDate)}</td>
                  <td>{formatDate(request.endDate)}</td>
                  <td>{request.numberOfDays}</td>
                  <td>{request.reason}</td>
                  <td>{request.status}</td>
                  <td>{request.reviewerName || '-'}</td>
                  <td>
                    {request.status === 'Pending' ? (
                      <div>
                        <button
                          onClick={() => handleApprove(request.id)}
                          disabled={processingId !== null}
                        >
                          Approve
                        </button>

                        <input
                          placeholder="Rejection reason"
                          value={rejectionReasons[request.id] || ''}
                          onChange={(e) =>
                            setRejectionReasons({
                              ...rejectionReasons,
                              [request.id]: e.target.value
                            })
                          }
                        />

                        <button
                          onClick={() => handleReject(request.id)}
                          disabled={processingId !== null}
                        >
                          Reject
                        </button>
                      </div>
                    ) : (
                      '-'
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </section>

      <button onClick={onLogout}>Logout</button>
    </div>
  );
}

export default ManagerDashboard;