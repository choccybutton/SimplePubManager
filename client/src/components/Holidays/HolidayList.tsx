import React, { useEffect } from 'react';
import { useHolidays } from '../../hooks/useHolidays';
import { LoadingSpinner } from '../Common/LoadingSpinner';

export const HolidayList: React.FC = () => {
  const { holidays, loading, error, fetchHolidays } = useHolidays();

  useEffect(() => {
    fetchHolidays({ pageNumber: 1, pageSize: 10 });
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  if (loading) {
    return <LoadingSpinner message="Loading holidays..." />;
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
      <h2>Holidays</h2>
      {holidays.length === 0 ? (
        <p>No holidays found.</p>
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
                Start Date
              </th>
              <th style={{ padding: '1rem', textAlign: 'left', borderBottom: '2px solid #ddd' }}>
                End Date
              </th>
              <th style={{ padding: '1rem', textAlign: 'left', borderBottom: '2px solid #ddd' }}>
                Status
              </th>
            </tr>
          </thead>
          <tbody>
            {holidays.map((holiday) => (
              <tr key={holiday.id} style={{ borderBottom: '1px solid #ddd' }}>
                <td style={{ padding: '1rem' }}>{holiday.staff?.name || 'Unknown'}</td>
                <td style={{ padding: '1rem' }}>
                  {new Date(holiday.startDate).toLocaleDateString()}
                </td>
                <td style={{ padding: '1rem' }}>
                  {new Date(holiday.endDate).toLocaleDateString()}
                </td>
                <td style={{ padding: '1rem' }}>
                  <span
                    style={{
                      padding: '0.25rem 0.75rem',
                      backgroundColor:
                        holiday.status === 'Approved'
                          ? '#e8f8e8'
                          : holiday.status === 'Rejected'
                          ? '#f8e8e8'
                          : '#f8f8e8',
                      borderRadius: '0.25rem',
                      fontSize: '0.875rem',
                    }}
                  >
                    {holiday.status}
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
