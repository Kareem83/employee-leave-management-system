import { useState, useEffect } from 'react';
import {
  getAllLeaveRequests,
  approveLeaveRequest,
  rejectLeaveRequest
} from './api';

function AdminLeaveRequests({ token }) {
  const [requests, setRequests] = useState([]);
  const [filter, setFilter] = useState('All');
  const [reasons, setReasons] = useState({});
  const [error, setError] = useState('');
  const [message, setMessage] = useState('');
  const [loading, setLoading] = useState(true);
  const [processingId, setProcessingId] = useState(null);

  useEffect(() => {
    async function load() {
      try {
        setRequests(await getAllLeaveRequests(token));
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    }

    load();
  }, [token]);

  async function refreshRequests() {
    try {
      setError('');
      setRequests(await getAllLeaveRequests(token));
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
      setMessage('Request approved successfully');
    } catch (err) {
      setError(err.message);
    } finally {
      setProcessingId(null);
    }
  }

  async function handleReject(id) {
    const reason = (reasons[id] || '').trim();

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
      setMessage('Request rejected successfully');
    } catch (err) {
      setError(err.message);
    } finally {
      setProcessingId(null);
    }
  }

  function formatDate(value) {
    return value ? value.split('T')[0] : '-';
  }

  const filteredRequests = requests.filter(
    (request) =>
      filter === 'All' || request.status === filter
  );

  return (
    <section id="all-requests">
      <h3>All Leave Requests</h3>

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
        Refresh Requests
      </button>

      {error && <p className="error-message">{error}</p>}
      {message && <p className="success-message">{message}</p>}

      {loading ? (
        <p>Loading requests...</p>
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
                        value={reasons[request.id] || ''}
                        onChange={(e) =>
                          setReasons({
                            ...reasons,
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
  );
}

export default AdminLeaveRequests;