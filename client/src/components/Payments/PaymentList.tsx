import React, { useState, useEffect } from 'react';
import { LoadingSpinner } from '../Common/LoadingSpinner';
import { paymentsApi } from '../../services/api/paymentsApi';

export const PaymentList: React.FC = () => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [payments, setPayments] = useState([]);

  useEffect(() => {
    const fetchPayments = async () => {
      setLoading(true);
      setError(null);
      try {
        const response = await paymentsApi.getPayments({ pageNumber: 1, pageSize: 10 });
        setPayments(response.data || []);
      } catch (err: any) {
        setError(err.message || 'Failed to fetch payments');
      } finally {
        setLoading(false);
      }
    };

    fetchPayments();
  }, []);

  if (loading) {
    return <LoadingSpinner message="Loading payments..." />;
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
      <h2>Payments</h2>
      {payments.length === 0 ? (
        <p>No payments found.</p>
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
                Amount
              </th>
              <th style={{ padding: '1rem', textAlign: 'left', borderBottom: '2px solid #ddd' }}>
                Type
              </th>
              <th style={{ padding: '1rem', textAlign: 'left', borderBottom: '2px solid #ddd' }}>
                Status
              </th>
              <th style={{ padding: '1rem', textAlign: 'left', borderBottom: '2px solid #ddd' }}>
                Created
              </th>
            </tr>
          </thead>
          <tbody>
            {payments.map((payment: any) => (
              <tr key={payment.id} style={{ borderBottom: '1px solid #ddd' }}>
                <td style={{ padding: '1rem' }}>${payment.amount.toFixed(2)}</td>
                <td style={{ padding: '1rem' }}>{payment.type}</td>
                <td style={{ padding: '1rem' }}>
                  <span
                    style={{
                      padding: '0.25rem 0.75rem',
                      backgroundColor: '#e8f4f8',
                      borderRadius: '0.25rem',
                      fontSize: '0.875rem',
                    }}
                  >
                    {payment.status}
                  </span>
                </td>
                <td style={{ padding: '1rem' }}>
                  {new Date(payment.createdAt).toLocaleDateString()}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
};
