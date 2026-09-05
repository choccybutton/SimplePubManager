import axiosInstance from './axiosConfig';
import { LoginRequest, LoginResponse, QuickSwapRequest, QuickSwapResponse } from '../../types/api';

export const authApi = {
  login: async (email: string, password: string): Promise<LoginResponse> => {
    // API resolves tenant from subdomain (Host header), no organizationId needed
    const request: LoginRequest = { email, password };
    const response = await axiosInstance.post<any>('/auth/login', request);

    // Extract the actual auth data from ApiResponse wrapper
    const authData = response.data.data;

    // Map the API response to LoginResponse format
    return {
      token: authData.token,
      user: {
        id: authData.userId,
        organizationId: authData.organizationId,
        name: authData.userName,
        email: authData.email,
        role: authData.role,
        status: 'Active', // Default from API response
        createdAt: new Date().toISOString(), // Use current date since API doesn't provide it
      },
    };
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
