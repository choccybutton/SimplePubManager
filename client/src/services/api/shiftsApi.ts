import axiosInstance from './axiosConfig';
import {
  Shift,
  GetShiftsParams,
  CreateShiftRequest,
  UpdateShiftRequest,
  ClockInRequest,
  ClockOutRequest,
  ApproveShiftRequest,
  PaginatedResponse,
} from '../../types/api';

/**
 * Shifts API client
 * Note: OrganizationId is resolved from subdomain by backend middleware
 */
export const shiftsApi = {
  getShifts: async (params: GetShiftsParams): Promise<PaginatedResponse<Shift>> => {
    const response = await axiosInstance.get<PaginatedResponse<Shift>>('/shifts', {
      params,
    });
    return response.data;
  },

  getShift: async (shiftId: string): Promise<Shift> => {
    const response = await axiosInstance.get<Shift>(`/shifts/${shiftId}`);
    return response.data;
  },

  createShift: async (data: CreateShiftRequest): Promise<Shift> => {
    const response = await axiosInstance.post<Shift>('/shifts', data);
    return response.data;
  },

  updateShift: async (shiftId: string, data: UpdateShiftRequest): Promise<Shift> => {
    const response = await axiosInstance.put<Shift>(`/shifts/${shiftId}`, data);
    return response.data;
  },

  deleteShift: async (shiftId: string): Promise<void> => {
    await axiosInstance.delete(`/shifts/${shiftId}`);
  },

  clockIn: async (shiftId: string): Promise<Shift> => {
    const request: ClockInRequest = { shiftId };
    const response = await axiosInstance.post<Shift>(`/shifts/${shiftId}/clock-in`, request);
    return response.data;
  },

  clockOut: async (shiftId: string): Promise<Shift> => {
    const request: ClockOutRequest = { shiftId };
    const response = await axiosInstance.post<Shift>(`/shifts/${shiftId}/clock-out`, request);
    return response.data;
  },

  approveShift: async (shiftId: string, data: ApproveShiftRequest): Promise<Shift> => {
    const response = await axiosInstance.post<Shift>(`/shifts/${shiftId}/approve`, data);
    return response.data;
  },

  getShiftLogs: async (shiftId: string) => {
    const response = await axiosInstance.get(`/shifts/${shiftId}/logs`);
    return response.data;
  },
};
