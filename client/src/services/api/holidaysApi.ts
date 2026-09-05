import axiosInstance from './axiosConfig';
import {
  Holiday,
  GetHolidaysParams,
  CreateHolidayRequest,
  ApproveHolidayRequest,
  PaginatedResponse,
} from '../../types/api';

export const holidaysApi = {
  getHolidays: async (params: GetHolidaysParams): Promise<PaginatedResponse<Holiday>> => {
    const response = await axiosInstance.get<any>('/holidays', {
      params,
    });
    return response.data.data;  // Unwrap ApiResponse to get PaginatedResponse
  },

  getHoliday: async (holidayId: string): Promise<Holiday> => {
    const response = await axiosInstance.get<Holiday>(`/holidays/${holidayId}`);
    return response.data;
  },

  createHoliday: async (data: CreateHolidayRequest): Promise<Holiday> => {
    const response = await axiosInstance.post<Holiday>('/holidays', data);
    return response.data;
  },

  updateHoliday: async (holidayId: string, data: Partial<CreateHolidayRequest>): Promise<Holiday> => {
    const response = await axiosInstance.put<Holiday>(`/holidays/${holidayId}`, data);
    return response.data;
  },

  deleteHoliday: async (holidayId: string): Promise<void> => {
    await axiosInstance.delete(`/holidays/${holidayId}`);
  },

  approveHoliday: async (holidayId: string, approve: boolean, notes?: string): Promise<Holiday> => {
    const request: ApproveHolidayRequest = { holidayId, approve, notes };
    const response = await axiosInstance.post<Holiday>(`/holidays/${holidayId}/approve`, request);
    return response.data;
  },

  rejectHoliday: async (holidayId: string, notes?: string): Promise<Holiday> => {
    const response = await axiosInstance.post<Holiday>(`/holidays/${holidayId}/reject`, {
      notes,
    });
    return response.data;
  },

  getPendingHolidays: async (params: GetHolidaysParams): Promise<PaginatedResponse<Holiday>> => {
    return holidaysApi.getHolidays({
      ...params,
      status: 'Pending',
    });
  },
};
