import { UserRole } from '../../types';

export const roleChecker = {
  isManager: (role: UserRole | null): boolean => {
    return role === UserRole.Manager;
  },

  isSupervisor: (role: UserRole | null): boolean => {
    return role === UserRole.Supervisor;
  },

  isStaff: (role: UserRole | null): boolean => {
    return role === UserRole.Staff;
  },

  isManagerOrSupervisor: (role: UserRole | null): boolean => {
    return role === UserRole.Manager || role === UserRole.Supervisor;
  },

  canApproveShifts: (role: UserRole | null): boolean => {
    return role === UserRole.Manager || role === UserRole.Supervisor;
  },

  canApproveTasks: (role: UserRole | null): boolean => {
    return role === UserRole.Manager || role === UserRole.Supervisor;
  },

  canApproveHolidays: (role: UserRole | null): boolean => {
    return role === UserRole.Manager || role === UserRole.Supervisor;
  },

  canViewStaff: (role: UserRole | null): boolean => {
    return role === UserRole.Manager || role === UserRole.Supervisor;
  },

  canManageAreas: (role: UserRole | null): boolean => {
    return role === UserRole.Manager;
  },

  canViewPayments: (role: UserRole | null): boolean => {
    return role === UserRole.Manager || role === UserRole.Supervisor;
  },
};
