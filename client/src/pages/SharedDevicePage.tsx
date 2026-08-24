import React, { useState } from 'react';
import { SharedDeviceLayout } from '../components/Layout/SharedDeviceLayout';
import { PinQuickSwap } from '../components/Auth/PinQuickSwap';

export const SharedDevicePage: React.FC = () => {
  const [userId, setUserId] = useState('');

  return (
    <SharedDeviceLayout>
      <div style={{ padding: '2rem', textAlign: 'center' }}>
        <h2>Welcome to Shared Device</h2>
        <p>Enter your user ID to begin.</p>
        <input
          type="text"
          value={userId}
          onChange={(e) => setUserId(e.target.value)}
          placeholder="Enter user ID"
          style={{
            padding: '0.75rem',
            fontSize: '1rem',
            marginBottom: '1rem',
            width: '100%',
            maxWidth: '400px',
            boxSizing: 'border-box',
          }}
        />
        {userId && (
          <PinQuickSwap
            userId={userId}
            onSuccess={() => {
              console.log('Quick swap successful');
            }}
          />
        )}
      </div>
    </SharedDeviceLayout>
  );
};
