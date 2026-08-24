import React from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';

export const Navigation: React.FC = () => {
  const { user, logout, isAuthenticated } = useAuth();

  const handleLogout = () => {
    logout();
    window.location.href = '/login';
  };

  return (
    <nav
      style={{
        backgroundColor: '#2c3e50',
        color: 'white',
        padding: '1rem',
        display: 'flex',
        justifyContent: 'space-between',
        alignItems: 'center',
      }}
    >
      <div style={{ fontSize: '1.5rem', fontWeight: 'bold' }}>
        <Link to="/" style={{ color: 'white', textDecoration: 'none' }}>
          SimplePubManager
        </Link>
      </div>

      {isAuthenticated && (
        <div style={{ display: 'flex', gap: '2rem', alignItems: 'center' }}>
          <div style={{ display: 'flex', gap: '1rem' }}>
            <Link to="/dashboard" style={{ color: 'white', textDecoration: 'none' }}>
              Dashboard
            </Link>
            <Link to="/shifts" style={{ color: 'white', textDecoration: 'none' }}>
              Shifts
            </Link>
            <Link to="/tasks" style={{ color: 'white', textDecoration: 'none' }}>
              Tasks
            </Link>
            <Link to="/holidays" style={{ color: 'white', textDecoration: 'none' }}>
              Holidays
            </Link>
          </div>

          <div style={{ display: 'flex', gap: '1rem', alignItems: 'center' }}>
            <span>{user?.name}</span>
            <button
              onClick={handleLogout}
              style={{
                padding: '0.5rem 1rem',
                backgroundColor: '#e74c3c',
                color: 'white',
                border: 'none',
                borderRadius: '0.25rem',
                cursor: 'pointer',
              }}
            >
              Logout
            </button>
          </div>
        </div>
      )}
    </nav>
  );
};
