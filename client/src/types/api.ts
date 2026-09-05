import { User, Shift, Task, Holiday, Bill, Payment, Area, Device } from './index';

// Request/Response DTOs

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  user: User;
}

export interface QuickSwapRequest {
  userId: string;
  pin: string;
}

export interface QuickSwapResponse {
  sessionToken: string;
  expiresAt: string;
  user: User;
}

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface ApiResponse<T> {
  data: T;
  message: string;
  success: boolean;
}

export interface ErrorResponse {
  message: string;
  errors?: Record<string, string[]>;
  statusCode: number;
}

// Shift DTOs
export interface GetShiftsParams {
  staffId?: string;
  status?: string;
  startDate?: string;
  endDate?: string;
  pageNumber?: number;
  pageSize?: number;
}

export interface CreateShiftRequest {
  staffId: string;
  type: string;
  startTime: string;
  endTime?: string;
  areaIds?: string[];
}

export interface UpdateShiftRequest {
  staffId?: string;
  status?: string;
  endTime?: string;
  areaIds?: string[];
}

export interface ClockInRequest {
  shiftId: string;
}

export interface ClockOutRequest {
  shiftId: string;
}

export interface ApproveShiftRequest {
  shiftId: string;
  adjustedStartTime?: string;
  adjustedEndTime?: string;
  notes?: string;
}

// Task DTOs
export interface GetTasksParams {
  assignedToUserId?: string;
  assignedToAreaId?: string;
  status?: string;
  pageNumber?: number;
  pageSize?: number;
}

export interface CreateTaskRequest {
  title: string;
  description?: string;
  assignedToUserId?: string;
  assignedToAreaId?: string;
  dueDate: string;
}

export interface UpdateTaskRequest {
  title?: string;
  description?: string;
  assignedToUserId?: string;
  assignedToAreaId?: string;
  dueDate?: string;
  status?: string;
}

export interface CompleteTaskRequest {
  completionNotes?: string;
}

// Holiday DTOs
export interface GetHolidaysParams {
  staffId?: string;
  status?: string;
  pageNumber?: number;
  pageSize?: number;
}

export interface CreateHolidayRequest {
  startDate: string;
  endDate: string;
  type: string;
}

export interface ApproveHolidayRequest {
  holidayId: string;
  approve: boolean;
  notes?: string;
}

// Payment/Bill DTOs
export interface GetPaymentsParams {
  status?: string;
  pageNumber?: number;
  pageSize?: number;
}

export interface CreateBillRequest {
  description: string;
  amount: number;
  dueDate: string;
}

export interface RecordPaymentRequest {
  amount: number;
  type: string;
}

// Staff DTOs
export interface GetStaffParams {
  status?: string;
  role?: string;
  pageNumber?: number;
  pageSize?: number;
}

export interface UpdateStaffRequest {
  name?: string;
  email?: string;
  role?: string;
  status?: string;
}

// Area DTOs
export interface CreateAreaRequest {
  name: string;
  description?: string;
}

export interface UpdateAreaRequest {
  name?: string;
  description?: string;
}

// Device DTOs
export interface CreateDeviceSessionRequest {
  deviceId: string;
}

export interface GetDeviceSessionParams {
  deviceId?: string;
  active?: boolean;
}
