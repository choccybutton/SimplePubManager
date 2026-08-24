import { useState } from 'react';
import { Task, GetTasksParams } from '../types/api';
import { tasksApi } from '../services/api/tasksApi';
import { PaginatedResponse } from '../types/api';

interface UseTasksReturn {
  tasks: Task[];
  loading: boolean;
  error: string | null;
  totalCount: number;
  currentPage: number;
  pageSize: number;
  fetchTasks: (params: GetTasksParams) => Promise<void>;
  completeTask: (taskId: string, notes?: string) => Promise<void>;
  assignTask: (taskId: string, userId: string) => Promise<void>;
  unassignTask: (taskId: string) => Promise<void>;
}

export const useTasks = (): UseTasksReturn => {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [totalCount, setTotalCount] = useState(0);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);

  const fetchTasks = async (params: GetTasksParams) => {
    setLoading(true);
    setError(null);
    try {
      const response: PaginatedResponse<Task> = await tasksApi.getTasks(params);
      setTasks(response.data);
      setTotalCount(response.totalCount);
      setCurrentPage(response.pageNumber);
      setPageSize(response.pageSize);
    } catch (err: any) {
      setError(err.message || 'Failed to fetch tasks');
    } finally {
      setLoading(false);
    }
  };

  const completeTask = async (taskId: string, notes?: string) => {
    try {
      await tasksApi.completeTask(taskId, { completionNotes: notes });
      await fetchTasks({ pageNumber: currentPage, pageSize });
    } catch (err: any) {
      setError(err.message || 'Failed to complete task');
    }
  };

  const assignTask = async (taskId: string, userId: string) => {
    try {
      await tasksApi.assignTask(taskId, userId);
      await fetchTasks({ pageNumber: currentPage, pageSize });
    } catch (err: any) {
      setError(err.message || 'Failed to assign task');
    }
  };

  const unassignTask = async (taskId: string) => {
    try {
      await tasksApi.unassignTask(taskId);
      await fetchTasks({ pageNumber: currentPage, pageSize });
    } catch (err: any) {
      setError(err.message || 'Failed to unassign task');
    }
  };

  return {
    tasks,
    loading,
    error,
    totalCount,
    currentPage,
    pageSize,
    fetchTasks,
    completeTask,
    assignTask,
    unassignTask,
  };
};
