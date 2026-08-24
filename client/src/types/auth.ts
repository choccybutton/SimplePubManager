import { User, UserRole } from './index';

export interface AuthState {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;
}

export interface AuthContextType {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;
  login: (email: string, password: string) => Promise<void>;
  logout: () => void;
  clearError: () => void;
  role: UserRole | null;
}

export interface DeviceSessionState {
  deviceId: string | null;
  sessionToken: string | null;
  user: User | null;
  isSharedDevice: boolean;
  isLoading: boolean;
  error: string | null;
}

export interface DeviceContextType {
  deviceId: string | null;
  sessionToken: string | null;
  user: User | null;
  isSharedDevice: boolean;
  isLoading: boolean;
  error: string | null;
  quickSwap: (userId: string, pin: string) => Promise<void>;
  logout: () => void;
  clearError: () => void;
}
