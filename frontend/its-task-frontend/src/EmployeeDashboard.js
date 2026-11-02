import { useState, useEffect } from 'react';
import {
  getMyProfile,
  getMyBalances,
  getLeaveTypes,
  getMyLeaveRequests,
  cancelLeaveRequest
} from './api';
import LeaveRequestForm from './LeaveRequestForm';

function EmployeeDashboard({ user, token, onLogout }) {
  const [profile, setProfile] = useState(null);
  const [profileError, setProfileError] = useState('');

  const [balances, setBalances] = useState([]);
  const [leaveTypes, setLeaveTypes] = useState([]);
  const [balancesLoading, setBalancesLoading] = useState(true);
  const [balancesError, setBalancesError] = useState('');

  const [requests, setRequests] = useState([]);
  const [requestsLoading, setRequestsLoading] = useState(true);
  const [requestsError, setRequestsError] = useState('');

  const [statusFilter, setStatusFilter] = useState('All');
  const [refreshing, setRefreshing] = useState(false);
  const [cancellingId, setCancellingId] = useState(null);
  const [message, setMessage] = useState('');

  useEffect(() => {
    async function loadProfile() {
      try {
        setProfile(await getMyProfile(token));
      } catch (err) {
        setProfileError(err.message);
      }
    }

    loadProfile();
  }, [token]);

  useEffect(() => {
    async function loadData() {
      try {
        const [balancesData, typesData, requestsData] =
          await Promise.all([
            getMyBalances(token),
            getLeaveTypes(token),
            getMyLeaveRequests(token)
          ]);

        setBalances(balancesData);
        setLeaveTypes(typesData);
        setRequests(requestsData);
      } catch (err) {
        setBalancesError(err.message);
        setRequestsError(err.message);
      } finally {
        setBalancesLoading(false);
        setRequestsLoading(false);
      }
    }

    loadData();
  }, [token]);

  async function refreshLeaveData() {
    setRefreshing(true);

    try {
      const [balancesData, requestsData] =
        await Promise.all([
          getMyBalances(token),
          getMyLeaveRequests(token)
        ]);

      setBalances(balancesData);
      setRequests(requestsData);
      setBalancesError('');
      setRequestsError('');
    } catch (err) {
      setBalancesError(err.message);
      setRequestsError(err.message);
    } finally {
      setRefreshing(false);
    }
  }

  async function handleCancel(id) {
    if (!window.confirm('Cancel this leave request?')) {
      return;
    }

    setCancellingId(id);
    setMessage('');
    setRequestsError('');

    try {
      await cancelLeaveRequest(token, id);
      setMessage('Leave request cancelled successfully');
      await refreshLeaveData();
    } catch (err) {
      setRequestsError(err.message);
    } finally {
      setCancellingId(null);
    }
  }

  function getLeaveTypeName(id) {
    const type = leaveTypes.find((item) => item.id === id);
    return type ? type.name : id;
  }

  function formatDate(value) {
    return value ? value.split('T')[0] : '-';
  }

  const filteredRequests = requests.filter(
    (request) =>
      statusFilter === 'All' ||
      request.status === statusFilter
  );

  return (
    <div>
      <h2>Employee Dashboard</h2>
      <p>Welcome: {user.email}</p>

      <section id="profile">
        <h3>My Profile</h3>

        {profileError && (
          <p className="error-message">{profileError}</p>
        )}

        {!profile && !profileError && (
          <p>Loading profile...</p>
        )}

        {profile && (
          <div className="profile-grid">
            <div><span>Name</span><strong>{profile.fullName}</strong></div>
            <div><span>Email</span><strong>{profile.email}</strong></div>
            <div><span>Department</span><strong>{profile.department}</strong></div>
            <div><span>Job Title</span><strong>{profile.jobTitle}</strong></div>
            <div><span>Hire Date</span><strong>{formatDate(profile.hireDate)}</strong></div>
          </div>
        )}
      </section>

      <section id="balances">
        <h3>My Leave Balances</h3>

        {balancesLoading ? (
          <p>Loading balances...</p>
        ) : balancesError ? (
          <p className="error-message">{balancesError}</p>
        ) : balances.length === 0 ? (
          <p>No leave balances available.</p>
        ) : (
          <table>
            <thead>
              <tr>
                <th>Leave Type</th>
                <th>Year</th>
                <th>Allocated</th>
                <th>Used</th>
                <th>Pending</th>
                <th>Remaining</th>
                <th>Available</th>
              </tr>
            </thead>

            <tbody>
              {balances.map((balance) => (
                <tr key={balance.id}>
                  <td>{getLeaveTypeName(balance.leaveTypeId)}</td>
                  <td>{balance.year}</td>
                  <td>{balance.allocatedDays}</td>
                  <td>{balance.usedDays}</td>
                  <td>{balance.pendingDays}</td>
                  <td>{balance.remainingDays}</td>
                  <td>{balance.availableDays}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </section>

      <section id="request-form">
        <LeaveRequestForm
          token={token}
          leaveTypes={leaveTypes}
          onRequestSubmitted={refreshLeaveData}
        />
      </section>

      <section id="requests">
        <h3>My Leave Requests</h3>

        <label>Filter by Status: </label>
        <select
          value={statusFilter}
          onChange={(e) => setStatusFilter(e.target.value)}
        >
          <option value="All">All</option>
          <option value="Pending">Pending</option>
          <option value="Approved">Approved</option>
          <option value="Rejected">Rejected</option>
          <option value="Cancelled">Cancelled</option>
        </select>

        <button
          onClick={refreshLeaveData}
          disabled={refreshing || cancellingId !== null}
        >
          {refreshing ? 'Refreshing...' : 'Refresh'}
        </button>

        {message && (
          <p className="success-message">{message}</p>
        )}

        {requestsLoading ? (
          <p>Loading requests...</p>
        ) : requestsError ? (
          <p className="error-message">{requestsError}</p>
        ) : filteredRequests.length === 0 ? (
          <p>No leave requests found.</p>
        ) : (
          <table>
            <thead>
              <tr>
                <th>ID</th>
                <th>Leave Type</th>
                <th>Start</th>
                <th>End</th>
                <th>Days</th>
                <th>Reason</th>
                <th>Status</th>
                <th>Reviewed By</th>
                <th>Rejection Reason</th>
                <th>Actions</th>
              </tr>
            </thead>

            <tbody>
              {filteredRequests.map((request) => (
                <tr key={request.id}>
                  <td>{request.id}</td>
                  <td>{request.leaveTypeName}</td>
                  <td>{formatDate(request.startDate)}</td>
                  <td>{formatDate(request.endDate)}</td>
                  <td>{request.numberOfDays}</td>
                  <td>{request.reason}</td>
                  <td>
                    <span className={`status-badge status-${request.status.toLowerCase()}`}>
                      {request.status}
                    </span>
                  </td>
                  <td>{request.reviewerName || '-'}</td>
                  <td>{request.rejectionReason || '-'}</td>
                  <td>
                    {request.status === 'Pending' ? (
                      <button
                        onClick={() => handleCancel(request.id)}
                        disabled={cancellingId !== null || refreshing}
                      >
                        {cancellingId === request.id
                          ? 'Cancelling...'
                          : 'Cancel'}
                      </button>
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

export default EmployeeDashboard;