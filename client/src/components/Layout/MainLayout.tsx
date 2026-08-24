import React from 'react';
import { Navigation } from '../Common/Navigation';

interface MainLayoutProps {
  children: React.ReactNode;
}

export const MainLayout: React.FC<MainLayoutProps> = ({ children }) => {
  return (
    <div style={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
      <Navigation />
      <main style={{ flex: 1, padding: '0', backgroundColor: '#f5f5f5' }}>
        {children}
      </main>
      <footer
        style={{
          backgroundColor: '#2c3e50',
          color: 'white',
          textAlign: 'center',
          padding: '1rem',
          borderTop: '1px solid #1a252f',
        }}
      >
        <p>&copy; 2026 SimplePubManager. All rights reserved.</p>
      </footer>
    </div>
  );
};
