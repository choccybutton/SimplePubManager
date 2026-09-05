import { useState } from 'react';
import { Holiday, GetHolidaysParams } from '../types/api';
import { holidaysApi } from '../services/api/holidaysApi';
import { PaginatedResponse } from '../types/api';

interface UseHolidaysReturn {
  holidays: Holiday[];
  loading: boolean;
  error: string | null;
  totalCount: number;
  currentPage: number;
  pageSize: number;
  fetchHolidays: (params: GetHolidaysParams) => Promise<void>;
  approveHoliday: (holidayId: string, notes?: string) => Promise<void>;
  rejectHoliday: (holidayId: string, notes?: string) => Promise<void>;
}

export const useHolidays = (): UseHolidaysReturn => {
  const [holidays, setHolidays] = useState<Holiday[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [totalCount, setTotalCount] = useState(0);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);

  const fetchHolidays = async (params: GetHolidaysParams) => {
    setLoading(true);
    setError(null);
    try {
      const response: PaginatedResponse<Holiday> = await holidaysApi.getHolidays(params);
      setHolidays(response.items);
      setTotalCount(response.totalCount);
      setCurrentPage(response.page);
      setPageSize(response.pageSize);
    } catch (err: any) {
      setError(err.message || 'Failed to fetch holidays');
    } finally {
      setLoading(false);
    }
  };

  const approveHoliday = async (holidayId: string, notes?: string) => {
    try {
      await holidaysApi.approveHoliday(holidayId, true, notes);
      await fetchHolidays({ pageNumber: currentPage, pageSize });
    } catch (err: any) {
      setError(err.message || 'Failed to approve holiday');
    }
  };

  const rejectHoliday = async (holidayId: string, notes?: string) => {
    try {
      await holidaysApi.rejectHoliday(holidayId, notes);
      await fetchHolidays({ pageNumber: currentPage, pageSize });
    } catch (err: any) {
      setError(err.message || 'Failed to reject holiday');
    }
  };

  return {
    holidays,
    loading,
    error,
    totalCount,
    currentPage,
    pageSize,
    fetchHolidays,
    approveHoliday,
    rejectHoliday,
  };
};
