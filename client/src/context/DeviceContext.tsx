import React, { useState, useEffect, useCallback } from 'react';
import { User } from '../types';
import { DeviceContextType } from '../types/auth';
import { authApi } from '../services/api/authApi';
import { storage } from '../services/localStorage/storage';

export const DeviceContext = React.createContext<DeviceContextType | undefined>(undefined);

const DEVICE_ID_KEY = 'device_id';
const DEVICE_SESSION_TOKEN_KEY = 'device_session_token';
const DEVICE_USER_KEY = 'device_user';

export const DeviceProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [deviceId, setDeviceId] = useState<string | null>(null);
  const [sessionToken, setSessionToken] = useState<string | null>(null);
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Initialize from localStorage
  useEffect(() => {
    const storedDeviceId = storage.getItem(DEVICE_ID_KEY);
    const storedSessionToken = storage.getItem(DEVICE_SESSION_TOKEN_KEY);
    const storedUser = storage.getItem(DEVICE_USER_KEY);

    if (storedDeviceId && storedSessionToken && storedUser) {
      setDeviceId(storedDeviceId);
      setSessionToken(storedSessionToken);
      setUser(storedUser);
    }

    setIsLoading(false);
  }, []);

  const quickSwap = useCallback(async (userId: string, pin: string) => {
    setIsLoading(true);
    setError(null);
    try {
      const response = await authApi.quickSwap(userId, pin);
      storage.setItem(DEVICE_SESSION_TOKEN_KEY, response.sessionToken);
      storage.setItem(DEVICE_USER_KEY, response.user);
      setSessionToken(response.sessionToken);
      setUser(response.user);
    } catch (err: any) {
      const errorMessage = err.response?.data?.message || err.message || 'Quick swap failed';
      setError(errorMessage);
      throw err;
    } finally {
      setIsLoading(false);
    }
  }, []);

  const logout = useCallback(() => {
    storage.removeItem(DEVICE_SESSION_TOKEN_KEY);
    storage.removeItem(DEVICE_USER_KEY);
    setSessionToken(null);
    setUser(null);
    setError(null);
  }, []);

  const clearError = useCallback(() => {
    setError(null);
  }, []);

  const value: DeviceContextType = {
    deviceId,
    sessionToken,
    user,
    isSharedDevice: !!deviceId,
    isLoading,
    error,
    quickSwap,
    logout,
    clearError,
  };

  return <DeviceContext.Provider value={value}>{children}</DeviceContext.Provider>;
};
