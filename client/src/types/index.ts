// Enums
export enum UserRole {
  Manager = 'Manager',
  Supervisor = 'Supervisor',
  Staff = 'Staff'
}

export enum UserStatus {
  Active = 'Active',
  Inactive = 'Inactive',
  Suspended = 'Suspended'
}

export enum ShiftStatus {
  Scheduled = 'Scheduled',
  InProgress = 'InProgress',
  Completed = 'Completed',
  Cancelled = 'Cancelled'
}

export enum ShiftType {
  Planned = 'Planned',
  AdHoc = 'AdHoc'
}

export enum TaskStatus {
  Pending = 'Pending',
  InProgress = 'InProgress',
  Completed = 'Completed',
  Cancelled = 'Cancelled'
}

export enum HolidayStatus {
  Pending = 'Pending',
  Approved = 'Approved',
  Rejected = 'Rejected',
  Cancelled = 'Cancelled'
}

export enum PaymentStatus {
  Pending = 'Pending',
  Processed = 'Processed',
  Failed = 'Failed'
}

export enum PaymentType {
  Hourly = 'Hourly',
  Salary = 'Salary',
  Bonus = 'Bonus'
}

export enum BillStatus {
  Pending = 'Pending',
  Paid = 'Paid',
  Overdue = 'Overdue'
}

export enum ShiftLogStatus {
  ClockIn = 'ClockIn',
  ClockOut = 'ClockOut',
  Break = 'Break'
}

// Entity Types
export interface User {
  id: string;
  organizationId: string;
  name: string;
  email: string;
  role: UserRole;
  status: UserStatus;
  createdAt: string;
}

export interface Area {
  id: string;
  organizationId: string;
  name: string;
  description?: string;
  createdAt: string;
}

export interface Shift {
  id: string;
  organizationId: string;
  staffId: string;
  type: ShiftType;
  startTime: string;
  endTime?: string;
  status: ShiftStatus;
  createdBy: string;
  createdAt: string;
  staff?: User;
  areas?: ShiftArea[];
  payment?: ShiftPayment;
}

export interface ShiftArea {
  id: string;
  shiftId: string;
  areaId: string;
  area?: Area;
}

export interface ShiftLog {
  id: string;
  shiftId: string;
  timestamp: string;
  status: ShiftLogStatus;
}

export interface ShiftPayment {
  id: string;
  shiftId: string;
  amount: number;
  paymentType: PaymentType;
  status: PaymentStatus;
  createdAt: string;
}

export interface Task {
  id: string;
  organizationId: string;
  title: string;
  description?: string;
  assignedToUserId?: string;
  assignedToAreaId?: string;
  dueDate: string;
  status: TaskStatus;
  completedBy?: string;
  completedAt?: string;
  completionNotes?: string;
  createdAt: string;
  assignedUser?: User;
  assignedArea?: Area;
}

export interface Holiday {
  id: string;
  staffId: string;
  organizationId: string;
  startDate: string;
  endDate: string;
  type: string;
  status: HolidayStatus;
  requestedAt: string;
  approvedBy?: string;
  approvedAt?: string;
  staff?: User;
  approvedByUser?: User;
}

export interface Bill {
  id: string;
  organizationId: string;
  description: string;
  amount: number;
  status: BillStatus;
  dueDate: string;
  createdAt: string;
}

export interface Payment {
  id: string;
  organizationId: string;
  amount: number;
  status: PaymentStatus;
  type: PaymentType;
  createdAt: string;
}

export interface Device {
  id: string;
  organizationId: string;
  name: string;
  deviceType: string;
  isActive: boolean;
  createdAt: string;
}

export interface DeviceSession {
  id: string;
  deviceId: string;
  userId: string;
  sessionToken: string;
  expiresAt: string;
  createdAt: string;
  user?: User;
  device?: Device;
}

export interface Organization {
  id: string;
  name: string;
  email: string;
  createdAt: string;
}
