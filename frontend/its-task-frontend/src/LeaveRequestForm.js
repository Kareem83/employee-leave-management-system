import { useState } from 'react';
import { submitLeaveRequest } from './api';

function LeaveRequestForm({ token, leaveTypes, onRequestSubmitted }) {
  const [leaveTypeId, setLeaveTypeId] = useState('');
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');
  const [reason, setReason] = useState('');

  const [message, setMessage] = useState('');
  const [submitting, setSubmitting] = useState(false);

  async function handleSubmit(e) {
    e.preventDefault();

    if (endDate < startDate) {
      setMessage('End date cannot be before start date');
      return;
    }

    setSubmitting(true);
    setMessage('');

    try {
      await submitLeaveRequest(token, {
        leaveTypeId: Number(leaveTypeId),
        startDate,
        endDate,
        reason
      });

      setMessage('Leave request submitted successfully');

      setLeaveTypeId('');
      setStartDate('');
      setEndDate('');
      setReason('');

      await onRequestSubmitted();
    } catch (error) {
      setMessage(error.message);
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div>
      <h3>Submit Leave Request</h3>

      <form onSubmit={handleSubmit}>
        <div>
          <label>Leave Type</label>
          <select
            value={leaveTypeId}
            onChange={(e) => setLeaveTypeId(e.target.value)}
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
          <label>Start Date</label>
          <input
            type="date"
            value={startDate}
            onChange={(e) => setStartDate(e.target.value)}
            required
          />
        </div>

        <div>
          <label>End Date</label>
          <input
            type="date"
            value={endDate}
            min={startDate || undefined}
            onChange={(e) => setEndDate(e.target.value)}
            required
          />
        </div>

        <div>
          <label>Reason</label>
          <textarea
            value={reason}
            onChange={(e) => setReason(e.target.value)}
            rows="3"
            required
          />
        </div>

        <button type="submit" disabled={submitting}>
          {submitting ? 'Submitting...' : 'Submit Request'}
        </button>
      </form>

      {message && <p>{message}</p>}
    </div>
  );
}

export default LeaveRequestForm;