import axiosInstance from './axiosConfig';
import { Device, DeviceSession, CreateDeviceSessionRequest, GetDeviceSessionParams } from '../../types/api';

export const devicesApi = {
  getDevices: async (params?: any) => {
    const response = await axiosInstance.get('/devices', { params });
    return response.data;
  },

  getDevice: async (deviceId: string): Promise<Device> => {
    const response = await axiosInstance.get<Device>(`/devices/${deviceId}`);
    return response.data;
  },

  createDevice: async (data: {
    name: string;
    deviceType: string;
  }): Promise<Device> => {
    const response = await axiosInstance.post<Device>('/devices', data);
    return response.data;
  },

  updateDevice: async (deviceId: string, data: any): Promise<Device> => {
    const response = await axiosInstance.put<Device>(`/devices/${deviceId}`, data);
    return response.data;
  },

  deleteDevice: async (deviceId: string): Promise<void> => {
    await axiosInstance.delete(`/devices/${deviceId}`);
  },

  activateDevice: async (deviceId: string): Promise<Device> => {
    const response = await axiosInstance.post<Device>(`/devices/${deviceId}/activate`);
    return response.data;
  },

  deactivateDevice: async (deviceId: string): Promise<Device> => {
    const response = await axiosInstance.post<Device>(`/devices/${deviceId}/deactivate`);
    return response.data;
  },

  createSession: async (deviceId: string): Promise<DeviceSession> => {
    const request: CreateDeviceSessionRequest = { deviceId };
    const response = await axiosInstance.post<DeviceSession>(`/devices/${deviceId}/sessions`, request);
    return response.data;
  },

  getSessions: async (params: GetDeviceSessionParams) => {
    const response = await axiosInstance.get('/devices/sessions', { params });
    return response.data;
  },

  getSession: async (sessionId: string): Promise<DeviceSession> => {
    const response = await axiosInstance.get<DeviceSession>(`/devices/sessions/${sessionId}`);
    return response.data;
  },

  endSession: async (sessionId: string): Promise<void> => {
    await axiosInstance.post(`/devices/sessions/${sessionId}/end`);
  },

  validateSession: async (sessionId: string): Promise<DeviceSession> => {
    const response = await axiosInstance.get<DeviceSession>(
      `/devices/sessions/${sessionId}/validate`
    );
    return response.data;
  },
};
