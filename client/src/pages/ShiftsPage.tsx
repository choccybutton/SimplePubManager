import React, { useState, useEffect } from 'react';
import { ShiftList } from '../components/Shifts/ShiftList';
import { UrgentShiftActionCard } from '../components/Shifts/UrgentShiftActionCard';
import { CreateShiftForm } from '../components/Shifts/CreateShiftForm';
import { MainLayout } from '../components/Layout/MainLayout';
import { useShifts } from '../hooks/useShifts';
import { useAuth } from '../hooks/useAuth';

export const ShiftsPage: React.FC = () => {
  const { shifts, loading, error, fetchShifts, clockIn } = useShifts();
  const { user } = useAuth();
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [isClockingIn, setIsClockingIn] = useState(false);

  useEffect(() => {
    fetchShifts({ pageNumber: 1, pageSize: 20 });
  }, []);

  const handleClockIn = async (shiftId: string) => {
    setIsClockingIn(true);
    try {
      await clockIn(shiftId);
      // Shifts will be refetched automatically via the hook
    } finally {
      setIsClockingIn(false);
    }
  };

  const handleCreateSuccess = () => {
    setShowCreateForm(false);
    fetchShifts({ pageNumber: 1, pageSize: 20 });
  };

  const userShifts = shifts.filter(s => s.staffId === user?.id);

  return (
    <MainLayout>
      <div className="space-y-6">
        {/* Urgent Action Card */}
        <UrgentShiftActionCard
          userRole={user?.role || 'Staff'}
          userShifts={userShifts}
          onClockIn={handleClockIn}
          isLoading={isClockingIn}
        />

        {/* Create Shift Button */}
        {(user?.role === 'Manager' || user?.role === 'Supervisor') && !showCreateForm && (
          <button
            onClick={() => setShowCreateForm(true)}
            className="w-full px-6 py-4 bg-blue-500 hover:bg-blue-600 text-white font-bold text-xl rounded-lg shadow-lg transition-all"
          >
            + Create New Shift
          </button>
        )}

        {/* Create Form Modal */}
        {showCreateForm && (
          <div className="bg-white rounded-xl shadow-xl overflow-hidden">
            <CreateShiftForm
              onSuccess={handleCreateSuccess}
              onCancel={() => setShowCreateForm(false)}
            />
          </div>
        )}

        {/* Shifts List */}
        {!showCreateForm && (
          <>
            {error && (
              <div className="p-4 bg-red-100 border-2 border-red-500 rounded-lg">
                <p className="text-red-800 font-semibold">{error}</p>
              </div>
            )}
            <ShiftList />
          </>
        )}
      </div>
    </MainLayout>
  );
};
