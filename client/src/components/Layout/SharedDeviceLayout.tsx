import React from 'react';

interface SharedDeviceLayoutProps {
  children: React.ReactNode;
}

export const SharedDeviceLayout: React.FC<SharedDeviceLayoutProps> = ({ children }) => {
  return (
    <div
      style={{
        display: 'flex',
        flexDirection: 'column',
        minHeight: '100vh',
        backgroundColor: '#f5f5f5',
      }}
    >
      <header
        style={{
          backgroundColor: '#2c3e50',
          color: 'white',
          padding: '1rem',
          textAlign: 'center',
        }}
      >
        <h1>SimplePubManager - Shared Device</h1>
      </header>
      <main style={{ flex: 1, padding: '1rem' }}>
        {children}
      </main>
      <footer
        style={{
          backgroundColor: '#2c3e50',
          color: 'white',
          textAlign: 'center',
          padding: '1rem',
          borderTop: '1px solid #1a252f',
          fontSize: '0.875rem',
        }}
      >
        <p>Shared Device Session - Swipe to change user</p>
      </footer>
    </div>
  );
};
