import axiosInstance from './axiosConfig';
import { LoginRequest, LoginResponse, QuickSwapRequest, QuickSwapResponse } from '../../types/api';

export const authApi = {
  login: async (email: string, password: string): Promise<LoginResponse> => {
    // API resolves tenant from subdomain (Host header), no organizationId needed
    const request: LoginRequest = { email, password };
    const response = await axiosInstance.post<LoginResponse>('/auth/login', request);
    return response.data;
  },

  logout: async (): Promise<void> => {
    await axiosInstance.post('/auth/logout');
  },

  quickSwap: async (userId: string, pin: string): Promise<QuickSwapResponse> => {
    const request: QuickSwapRequest = { userId, pin };
    const response = await axiosInstance.post<QuickSwapResponse>('/auth/quick-swap', request);
    return response.data;
  },

  validateToken: async (): Promise<boolean> => {
    try {
      const response = await axiosInstance.get('/auth/validate');
      return response.status === 200;
    } catch {
      return false;
    }
  },

  refreshToken: async (): Promise<LoginResponse> => {
    const response = await axiosInstance.post<LoginResponse>('/auth/refresh');
    return response.data;
  },
};
