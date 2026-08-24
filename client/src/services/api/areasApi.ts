import axiosInstance from './axiosConfig';
import { Area, CreateAreaRequest, UpdateAreaRequest, PaginatedResponse } from '../../types/api';

export const areasApi = {
  getAreas: async (params?: any): Promise<PaginatedResponse<Area> | Area[]> => {
    const response = await axiosInstance.get('/areas', { params });
    return response.data;
  },

  getArea: async (areaId: string): Promise<Area> => {
    const response = await axiosInstance.get<Area>(`/areas/${areaId}`);
    return response.data;
  },

  createArea: async (data: CreateAreaRequest): Promise<Area> => {
    const response = await axiosInstance.post<Area>('/areas', data);
    return response.data;
  },

  updateArea: async (areaId: string, data: UpdateAreaRequest): Promise<Area> => {
    const response = await axiosInstance.put<Area>(`/areas/${areaId}`, data);
    return response.data;
  },

  deleteArea: async (areaId: string): Promise<void> => {
    await axiosInstance.delete(`/areas/${areaId}`);
  },

  getAreaTasks: async (areaId: string, params?: any) => {
    const response = await axiosInstance.get(`/areas/${areaId}/tasks`, { params });
    return response.data;
  },

  getAreaShifts: async (areaId: string, params?: any) => {
    const response = await axiosInstance.get(`/areas/${areaId}/shifts`, { params });
    return response.data;
  },
};
