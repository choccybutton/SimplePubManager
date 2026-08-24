import React from 'react';
import { useAuth } from '../../hooks/useAuth';
import { UserRole } from '../../types';

export const Dashboard: React.FC = () => {
  const { user, role } = useAuth();

  const isManager = role === UserRole.Manager;
  const isSupervisor = role === UserRole.Supervisor;

  return (
    <div style={{ padding: '2rem' }}>
      <h1>Welcome, {user?.name}!</h1>
      <p>Role: {role}</p>

      <div style={{ marginTop: '2rem', display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(250px, 1fr))', gap: '1rem' }}>
        <div
          style={{
            padding: '1.5rem',
            backgroundColor: '#ecf0f1',
            borderRadius: '0.5rem',
            boxShadow: '0 2px 4px rgba(0,0,0,0.1)',
          }}
        >
          <h3>Quick Actions</h3>
          <ul style={{ listStyle: 'none', padding: 0 }}>
            <li><a href="/shifts">View Shifts</a></li>
            <li><a href="/tasks">View Tasks</a></li>
            <li><a href="/holidays">Request Holiday</a></li>
            {(isManager || isSupervisor) && (
              <>
                <li><a href="/staff">Manage Staff</a></li>
                <li><a href="/areas">Manage Areas</a></li>
              </>
            )}
          </ul>
        </div>

        <div
          style={{
            padding: '1.5rem',
            backgroundColor: '#e8f4f8',
            borderRadius: '0.5rem',
            boxShadow: '0 2px 4px rgba(0,0,0,0.1)',
          }}
        >
          <h3>Recent Activity</h3>
          <p>Your recent shifts, tasks, and updates will appear here.</p>
        </div>

        {(isManager || isSupervisor) && (
          <div
            style={{
              padding: '1.5rem',
              backgroundColor: '#f0e8f4',
              borderRadius: '0.5rem',
              boxShadow: '0 2px 4px rgba(0,0,0,0.1)',
            }}
          >
            <h3>Pending Approvals</h3>
            <p>Shifts, holidays, and other items awaiting your approval.</p>
          </div>
        )}
      </div>
    </div>
  );
};
