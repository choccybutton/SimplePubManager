import React, { useState, useEffect } from 'react';
import { Shift } from '../../types/api';
import { ShiftForm } from './ShiftForm';
import { shiftsApi } from '../../services/api/shiftsApi';
import { staffApi } from '../../services/api/staffApi';
import { areasApi } from '../../services/api/areasApi';

interface EditShiftFormProps {
  shift: Shift;
  onSuccess: () => void;
  onCancel: () => void;
}

export const EditShiftForm: React.FC<EditShiftFormProps> = ({ shift, onSuccess, onCancel }) => {
  const [staffMembers, setStaffMembers] = useState<any[]>([]);
  const [areas, setAreas] = useState<any[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [dataLoading, setDataLoading] = useState(true);

  useEffect(() => {
    const loadData = async () => {
      try {
        const [staffRes, areasRes] = await Promise.all([
          staffApi.getStaff({ pageNumber: 1, pageSize: 100 }),
          areasApi.getAreas(),
        ]);
        setStaffMembers(Array.isArray(staffRes) ? staffRes : staffRes.items || []);
        setAreas(Array.isArray(areasRes) ? areasRes : areasRes.items || []);
      } catch (err: any) {
        setError('Failed to load staff and areas');
      } finally {
        setDataLoading(false);
      }
    };

    loadData();
  }, []);

  const handleSubmit = async (shiftData: any) => {
    setIsLoading(true);
    setError(null);
    try {
      await shiftsApi.updateShift(shift.id, {
        staffId: shiftData.staffId,
        startTime: shiftData.startTime,
        endTime: shiftData.endTime,
        areaIds: shiftData.areaIds,
      });
      onSuccess();
    } catch (err: any) {
      setError(err.message || 'Failed to update shift');
    } finally {
      setIsLoading(false);
    }
  };

  if (dataLoading) {
    return (
      <div className="flex items-center justify-center p-8">
        <p className="text-xl text-gray-600">Loading...</p>
      </div>
    );
  }

  return (
    <div>
      {error && (
        <div className="mb-4 p-4 bg-red-100 border-2 border-red-500 rounded-lg">
          <p className="text-red-800 font-semibold">{error}</p>
        </div>
      )}
      <ShiftForm
        initialShift={shift}
        staffMembers={staffMembers}
        areas={areas}
        onSubmit={handleSubmit}
        onCancel={onCancel}
        isLoading={isLoading}
      />
    </div>
  );
};
