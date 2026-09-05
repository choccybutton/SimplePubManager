import axiosInstance from './axiosConfig';
import {
  Task,
  GetTasksParams,
  CreateTaskRequest,
  UpdateTaskRequest,
  CompleteTaskRequest,
  PaginatedResponse,
} from '../../types/api';

export const tasksApi = {
  getTasks: async (params: GetTasksParams): Promise<PaginatedResponse<Task>> => {
    const response = await axiosInstance.get<any>('/tasks', {
      params,
    });
    return response.data.data;  // Unwrap ApiResponse to get PaginatedResponse
  },

  getTask: async (taskId: string): Promise<Task> => {
    const response = await axiosInstance.get<Task>(`/tasks/${taskId}`);
    return response.data;
  },

  createTask: async (data: CreateTaskRequest): Promise<Task> => {
    const response = await axiosInstance.post<Task>('/tasks', data);
    return response.data;
  },

  updateTask: async (taskId: string, data: UpdateTaskRequest): Promise<Task> => {
    const response = await axiosInstance.put<Task>(`/tasks/${taskId}`, data);
    return response.data;
  },

  deleteTask: async (taskId: string): Promise<void> => {
    await axiosInstance.delete(`/tasks/${taskId}`);
  },

  completeTask: async (taskId: string, data: CompleteTaskRequest): Promise<Task> => {
    const response = await axiosInstance.post<Task>(`/tasks/${taskId}/complete`, data);
    return response.data;
  },

  assignTask: async (taskId: string, userId: string): Promise<Task> => {
    const response = await axiosInstance.post<Task>(`/tasks/${taskId}/assign`, {
      userId,
    });
    return response.data;
  },

  unassignTask: async (taskId: string): Promise<Task> => {
    const response = await axiosInstance.post<Task>(`/tasks/${taskId}/unassign`);
    return response.data;
  },
};
