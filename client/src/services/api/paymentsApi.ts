import axiosInstance from './axiosConfig';
import {
  Payment,
  Bill,
  GetPaymentsParams,
  CreateBillRequest,
  RecordPaymentRequest,
  PaginatedResponse,
} from '../../types/api';

export const paymentsApi = {
  getPayments: async (params: GetPaymentsParams): Promise<PaginatedResponse<Payment>> => {
    const response = await axiosInstance.get<any>('/payments', {
      params,
    });
    return response.data.data;  // Unwrap ApiResponse to get PaginatedResponse
  },

  getPayment: async (paymentId: string): Promise<Payment> => {
    const response = await axiosInstance.get<Payment>(`/payments/${paymentId}`);
    return response.data;
  },

  recordPayment: async (data: RecordPaymentRequest): Promise<Payment> => {
    const response = await axiosInstance.post<Payment>('/payments', data);
    return response.data;
  },

  recordStaffPayment: async (staffId: string, data: RecordPaymentRequest): Promise<Payment> => {
    const response = await axiosInstance.post<Payment>(`/staff/${staffId}/payments`, data);
    return response.data;
  },

  getBills: async (params: GetPaymentsParams): Promise<PaginatedResponse<Bill>> => {
    const response = await axiosInstance.get<any>('/bills', {
      params,
    });
    return response.data.data;  // Unwrap ApiResponse to get PaginatedResponse
  },

  getBill: async (billId: string): Promise<Bill> => {
    const response = await axiosInstance.get<Bill>(`/bills/${billId}`);
    return response.data;
  },

  createBill: async (data: CreateBillRequest): Promise<Bill> => {
    const response = await axiosInstance.post<Bill>('/bills', data);
    return response.data;
  },

  updateBill: async (billId: string, data: Partial<CreateBillRequest>): Promise<Bill> => {
    const response = await axiosInstance.put<Bill>(`/bills/${billId}`, data);
    return response.data;
  },

  deleteBill: async (billId: string): Promise<void> => {
    await axiosInstance.delete(`/bills/${billId}`);
  },

  markBillAsPaid: async (billId: string): Promise<Bill> => {
    const response = await axiosInstance.post<Bill>(`/bills/${billId}/mark-paid`);
    return response.data;
  },

  getPaymentSummary: async () => {
    const response = await axiosInstance.get('/payments/summary');
    return response.data;
  },
};
