import React, { useState } from 'react';
import { useDeviceSession } from '../../hooks/useDeviceSession';

interface PinQuickSwapProps {
  userId: string;
  onSuccess?: () => void;
}

export const PinQuickSwap: React.FC<PinQuickSwapProps> = ({ userId, onSuccess }) => {
  const { quickSwap, error } = useDeviceSession();
  const [pin, setPin] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [localError, setLocalError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setLocalError(null);

    try {
      await quickSwap(userId, pin);
      if (onSuccess) {
        onSuccess();
      }
    } catch (err: any) {
      setLocalError(err.message || 'Quick swap failed');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <form
      onSubmit={handleSubmit}
      style={{
        maxWidth: '300px',
        margin: '0 auto',
        padding: '1.5rem',
        backgroundColor: '#f9f9f9',
        borderRadius: '0.5rem',
        boxShadow: '0 2px 4px rgba(0,0,0,0.1)',
      }}
    >
      <h3 style={{ textAlign: 'center', marginBottom: '1rem' }}>Enter PIN</h3>

      {(error || localError) && (
        <div
          style={{
            padding: '0.75rem',
            backgroundColor: '#fee',
            border: '1px solid #fcc',
            borderRadius: '0.25rem',
            marginBottom: '1rem',
            color: '#c00',
            fontSize: '0.875rem',
          }}
        >
          {error || localError}
        </div>
      )}

      <div style={{ marginBottom: '1rem' }}>
        <input
          type="password"
          value={pin}
          onChange={(e) => setPin(e.target.value)}
          placeholder="Enter PIN"
          maxLength="6"
          required
          style={{
            width: '100%',
            padding: '0.75rem',
            border: '1px solid #ddd',
            borderRadius: '0.25rem',
            boxSizing: 'border-box',
            fontSize: '1rem',
            textAlign: 'center',
            letterSpacing: '0.2rem',
          }}
        />
      </div>

      <button
        type="submit"
        disabled={isLoading || pin.length === 0}
        style={{
          width: '100%',
          padding: '0.75rem',
          backgroundColor: '#27ae60',
          color: 'white',
          border: 'none',
          borderRadius: '0.25rem',
          cursor: isLoading || pin.length === 0 ? 'not-allowed' : 'pointer',
          fontWeight: 'bold',
          opacity: isLoading || pin.length === 0 ? 0.7 : 1,
        }}
      >
        {isLoading ? 'Authenticating...' : 'Confirm'}
      </button>
    </form>
  );
};
