import { useState } from 'react';
import { Shift, GetShiftsParams } from '../types/api';
import { shiftsApi } from '../services/api/shiftsApi';
import { PaginatedResponse } from '../types/api';

interface UseShiftsReturn {
  shifts: Shift[];
  loading: boolean;
  error: string | null;
  totalCount: number;
  currentPage: number;
  pageSize: number;
  fetchShifts: (params: GetShiftsParams) => Promise<void>;
  clockIn: (shiftId: string) => Promise<void>;
  clockOut: (shiftId: string) => Promise<void>;
  approveShift: (shiftId: string, adjustedStartTime?: string, adjustedEndTime?: string) => Promise<void>;
}

export const useShifts = (): UseShiftsReturn => {
  const [shifts, setShifts] = useState<Shift[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [totalCount, setTotalCount] = useState(0);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);

  const fetchShifts = async (params: GetShiftsParams) => {
    setLoading(true);
    setError(null);
    try {
      const response: PaginatedResponse<Shift> = await shiftsApi.getShifts(params);
      setShifts(response.data);
      setTotalCount(response.totalCount);
      setCurrentPage(response.pageNumber);
      setPageSize(response.pageSize);
    } catch (err: any) {
      setError(err.message || 'Failed to fetch shifts');
    } finally {
      setLoading(false);
    }
  };

  const clockIn = async (shiftId: string) => {
    try {
      await shiftsApi.clockIn(shiftId);
      await fetchShifts({ pageNumber: currentPage, pageSize });
    } catch (err: any) {
      setError(err.message || 'Failed to clock in');
    }
  };

  const clockOut = async (shiftId: string) => {
    try {
      await shiftsApi.clockOut(shiftId);
      await fetchShifts({ pageNumber: currentPage, pageSize });
    } catch (err: any) {
      setError(err.message || 'Failed to clock out');
    }
  };

  const approveShift = async (shiftId: string, adjustedStartTime?: string, adjustedEndTime?: string) => {
    try {
      await shiftsApi.approveShift(shiftId, {
        shiftId,
        adjustedStartTime,
        adjustedEndTime,
      });
      await fetchShifts({ pageNumber: currentPage, pageSize });
    } catch (err: any) {
      setError(err.message || 'Failed to approve shift');
    }
  };

  return {
    shifts,
    loading,
    error,
    totalCount,
    currentPage,
    pageSize,
    fetchShifts,
    clockIn,
    clockOut,
    approveShift,
  };
};
