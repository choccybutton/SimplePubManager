import React, { useEffect } from 'react';
import { useShifts } from '../../hooks/useShifts';
import { LoadingSpinner } from '../Common/LoadingSpinner';

export const ShiftList: React.FC = () => {
  const { shifts, loading, error, fetchShifts } = useShifts();

  useEffect(() => {
    fetchShifts({ pageNumber: 1, pageSize: 10 });
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  if (loading) {
    return <LoadingSpinner message="Loading shifts..." />;
  }

  if (error) {
    return (
      <div style={{ padding: '2rem', color: 'red' }}>
        <h2>Error</h2>
        <p>{error}</p>
      </div>
    );
  }

  return (
    <div style={{ padding: '2rem' }}>
      <h2>Shifts</h2>
      {shifts.length === 0 ? (
        <p>No shifts found.</p>
      ) : (
        <table
          style={{
            width: '100%',
            borderCollapse: 'collapse',
            backgroundColor: 'white',
          }}
        >
          <thead>
            <tr style={{ backgroundColor: '#ecf0f1' }}>
              <th style={{ padding: '1rem', textAlign: 'left', borderBottom: '2px solid #ddd' }}>
                Staff
              </th>
              <th style={{ padding: '1rem', textAlign: 'left', borderBottom: '2px solid #ddd' }}>
                Start Time
              </th>
              <th style={{ padding: '1rem', textAlign: 'left', borderBottom: '2px solid #ddd' }}>
                End Time
              </th>
              <th style={{ padding: '1rem', textAlign: 'left', borderBottom: '2px solid #ddd' }}>
                Status
              </th>
            </tr>
          </thead>
          <tbody>
            {shifts.map((shift) => (
              <tr key={shift.id} style={{ borderBottom: '1px solid #ddd' }}>
                <td style={{ padding: '1rem' }}>{shift.staff?.name || 'Unknown'}</td>
                <td style={{ padding: '1rem' }}>
                  {new Date(shift.startTime).toLocaleString()}
                </td>
                <td style={{ padding: '1rem' }}>
                  {shift.endTime ? new Date(shift.endTime).toLocaleString() : 'N/A'}
                </td>
                <td style={{ padding: '1rem' }}>
                  <span
                    style={{
                      padding: '0.25rem 0.75rem',
                      backgroundColor: '#e8f4f8',
                      borderRadius: '0.25rem',
                      fontSize: '0.875rem',
                    }}
                  >
                    {shift.status}
                  </span>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
};
