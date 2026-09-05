import axiosInstance from './axiosConfig';
import { User, GetStaffParams, UpdateStaffRequest, PaginatedResponse } from '../../types/api';

export const staffApi = {
  getStaff: async (params: GetStaffParams): Promise<PaginatedResponse<User>> => {
    const response = await axiosInstance.get<any>('/staff', {
      params,
    });
    return response.data.data;  // Unwrap ApiResponse to get PaginatedResponse
  },

  getStaffMember: async (staffId: string): Promise<User> => {
    const response = await axiosInstance.get<User>(`/staff/${staffId}`);
    return response.data;
  },

  createStaff: async (data: {
    name: string;
    email: string;
    role: string;
    password?: string;
  }): Promise<User> => {
    const response = await axiosInstance.post<User>('/staff', data);
    return response.data;
  },

  updateStaff: async (staffId: string, data: UpdateStaffRequest): Promise<User> => {
    const response = await axiosInstance.put<User>(`/staff/${staffId}`, data);
    return response.data;
  },

  deleteStaff: async (staffId: string): Promise<void> => {
    await axiosInstance.delete(`/staff/${staffId}`);
  },

  activateStaff: async (staffId: string): Promise<User> => {
    const response = await axiosInstance.post<User>(`/staff/${staffId}/activate`);
    return response.data;
  },

  deactivateStaff: async (staffId: string): Promise<User> => {
    const response = await axiosInstance.post<User>(`/staff/${staffId}/deactivate`);
    return response.data;
  },

  getStaffShifts: async (staffId: string, params?: any) => {
    const response = await axiosInstance.get(`/staff/${staffId}/shifts`, { params });
    return response.data;
  },

  getStaffTasks: async (staffId: string, params?: any) => {
    const response = await axiosInstance.get(`/staff/${staffId}/tasks`, { params });
    return response.data;
  },

  getStaffHolidays: async (staffId: string, params?: any) => {
    const response = await axiosInstance.get(`/staff/${staffId}/holidays`, { params });
    return response.data;
  },

  createUserPin: async (staffId: string, pin: string) => {
    const response = await axiosInstance.post(`/staff/${staffId}/pins`, { pin });
    return response.data;
  },

  deleteUserPin: async (staffId: string, pinId: string): Promise<void> => {
    await axiosInstance.delete(`/staff/${staffId}/pins/${pinId}`);
  },
};
