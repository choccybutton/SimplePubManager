import { useContext } from 'react';
import { DeviceContext } from '../context/DeviceContext';
import { DeviceContextType } from '../types/auth';

export const useDeviceSession = (): DeviceContextType => {
  const context = useContext(DeviceContext);
  if (!context) {
    throw new Error('useDeviceSession must be used within a DeviceProvider');
  }
  return context;
};
