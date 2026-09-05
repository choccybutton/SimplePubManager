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
 * Note: All endpoints require organizationId to be passed as the first parameter
 */
export const shiftsApi = {
  getShifts: async (organizationId: string, params: GetShiftsParams): Promise<PaginatedResponse<Shift>> => {
    const response = await axiosInstance.get<PaginatedResponse<Shift>>(
      `/organizations/${organizationId}/shifts`,
      { params }
    );
    return response.data;
  },

  getShift: async (organizationId: string, shiftId: string): Promise<Shift> => {
    const response = await axiosInstance.get<Shift>(
      `/organizations/${organizationId}/shifts/${shiftId}`
    );
    return response.data;
  },

  createShift: async (organizationId: string, data: CreateShiftRequest): Promise<Shift> => {
    const response = await axiosInstance.post<Shift>(
      `/organizations/${organizationId}/shifts`,
      data
    );
    return response.data;
  },

  updateShift: async (organizationId: string, shiftId: string, data: UpdateShiftRequest): Promise<Shift> => {
    const response = await axiosInstance.put<Shift>(
      `/organizations/${organizationId}/shifts/${shiftId}`,
      data
    );
    return response.data;
  },

  deleteShift: async (organizationId: string, shiftId: string): Promise<void> => {
    await axiosInstance.delete(`/organizations/${organizationId}/shifts/${shiftId}`);
  },

  clockIn: async (organizationId: string, shiftId: string): Promise<Shift> => {
    const request: ClockInRequest = { shiftId };
    const response = await axiosInstance.post<Shift>(
      `/organizations/${organizationId}/shifts/${shiftId}/clock-in`,
      request
    );
    return response.data;
  },

  clockOut: async (organizationId: string, shiftId: string): Promise<Shift> => {
    const request: ClockOutRequest = { shiftId };
    const response = await axiosInstance.post<Shift>(
      `/organizations/${organizationId}/shifts/${shiftId}/clock-out`,
      request
    );
    return response.data;
  },

  approveShift: async (organizationId: string, shiftId: string, data: ApproveShiftRequest): Promise<Shift> => {
    const response = await axiosInstance.post<Shift>(
      `/organizations/${organizationId}/shifts/${shiftId}/approve`,
      data
    );
    return response.data;
  },

  getShiftLogs: async (organizationId: string, shiftId: string) => {
    const response = await axiosInstance.get(
      `/organizations/${organizationId}/shifts/${shiftId}/logs`
    );
    return response.data;
  },
};
