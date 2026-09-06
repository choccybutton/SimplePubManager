import React, { useState } from 'react';
import { Shift } from '../../types/api';

interface UrgentShiftActionCardProps {
  userRole: string; // 'Manager', 'Supervisor', 'Staff'
  userShifts?: Shift[];
  pendingApprovals?: number;
  onClockIn?: (shiftId: string) => void;
  onViewApprovals?: () => void;
  isLoading?: boolean;
}

export const UrgentShiftActionCard: React.FC<UrgentShiftActionCardProps> = ({
  userRole,
  userShifts = [],
  pendingApprovals = 0,
  onClockIn,
  onViewApprovals,
  isLoading = false,
}) => {
  // Check if staff member has shift starting soon
  const getUpcomingShift = () => {
    const now = new Date();
    const thirtyMinutesFromNow = new Date(now.getTime() + 30 * 60000);

    return userShifts?.find(shift => {
      const shiftStart = new Date(shift.startTime);
      return shiftStart >= now && shiftStart <= thirtyMinutesFromNow;
    });
  };

  const upcomingShift = userRole === 'Staff' || userRole === 'Supervisor' ? getUpcomingShift() : null;
  const hasUrgentAction = upcomingShift || (userRole === 'Manager' && pendingApprovals > 0);

  if (!hasUrgentAction) {
    return null; // Don't show if no urgent action
  }

  // Staff member with upcoming shift
  if (upcomingShift) {
    const shiftStart = new Date(upcomingShift.startTime);
    const minutesUntilShift = Math.round((shiftStart.getTime() - new Date().getTime()) / 60000);
    const timeDisplay = minutesUntilShift <= 0 ? 'NOW' : `in ${minutesUntilShift} minutes`;

    return (
      <div className="mb-6 p-6 bg-gradient-to-r from-red-500 to-red-600 rounded-xl shadow-2xl border-4 border-red-700">
        <div className="text-center">
          <p className="text-white text-lg font-semibold mb-1">⏰ CLOCK IN NOW</p>
          <p className="text-red-100 text-sm mb-4">Shift starts {timeDisplay}</p>

          <div className="bg-white rounded-lg p-4 mb-6">
            <p className="text-gray-900 font-bold text-lg">
              {shiftStart.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' })}
            </p>
            <p className="text-gray-600 text-sm">
              {upcomingShift.areaIds?.join(', ') || 'Assigned Areas'}
            </p>
          </div>

          <button
            onClick={() => onClockIn?.(upcomingShift.id)}
            disabled={isLoading}
            className="w-full px-8 py-4 bg-green-500 hover:bg-green-600 disabled:bg-gray-400 text-white font-bold text-xl rounded-lg shadow-lg transition-all transform hover:scale-105"
          >
            {isLoading ? '⏳ Clocking In...' : '✓ Clock In Now'}
          </button>
        </div>
      </div>
    );
  }

  // Manager with pending approvals
  if (userRole === 'Manager' && pendingApprovals > 0) {
    return (
      <div className="mb-6 p-6 bg-gradient-to-r from-orange-500 to-orange-600 rounded-xl shadow-2xl border-4 border-orange-700">
        <div className="text-center">
          <p className="text-white text-lg font-semibold mb-1">🔴 PENDING APPROVALS</p>
          <p className="text-orange-100 text-sm mb-4">Action Required</p>

          <div className="bg-white rounded-lg p-4 mb-6">
            <p className="text-gray-900 font-bold text-4xl">{pendingApprovals}</p>
            <p className="text-gray-600 text-sm">shift{pendingApprovals !== 1 ? 's' : ''} awaiting approval</p>
          </div>

          <button
            onClick={onViewApprovals}
            disabled={isLoading}
            className="w-full px-8 py-4 bg-blue-500 hover:bg-blue-600 disabled:bg-gray-400 text-white font-bold text-xl rounded-lg shadow-lg transition-all transform hover:scale-105"
          >
            {isLoading ? '⏳ Loading...' : '👁️ Review Approvals'}
          </button>
        </div>
      </div>
    );
  }

  return null;
};
