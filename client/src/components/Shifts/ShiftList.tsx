import React, { useEffect, useState } from 'react';
import { useShifts } from '../../hooks/useShifts';
import { LoadingSpinner } from '../Common/LoadingSpinner';
import { EditShiftForm } from './EditShiftForm';
import { Shift } from '../../types/api';

export const ShiftList: React.FC = () => {
  const { shifts, loading, error, fetchShifts } = useShifts();
  const [editingShiftId, setEditingShiftId] = useState<string | null>(null);

  useEffect(() => {
    fetchShifts({ pageNumber: 1, pageSize: 10 });
  }, []);

  const editingShift = shifts.find(s => s.id === editingShiftId);

  if (loading) {
    return <LoadingSpinner message="Loading shifts..." />;
  }

  if (error) {
    return (
      <div style={{ padding: '2rem', color: 'red' }} className="bg-red-50 border-2 border-red-500 rounded-lg">
        <h2 className="font-bold text-lg">Error</h2>
        <p>{error}</p>
      </div>
    );
  }

  if (editingShift && editingShiftId) {
    return (
      <div className="bg-white rounded-xl shadow-xl overflow-hidden">
        <EditShiftForm
          shift={editingShift}
          onSuccess={() => {
            setEditingShiftId(null);
            fetchShifts({ pageNumber: 1, pageSize: 10 });
          }}
          onCancel={() => setEditingShiftId(null)}
        />
      </div>
    );
  }

  return (
    <div style={{ padding: '2rem' }} className="space-y-4">
      <h2 className="text-3xl font-bold text-gray-900 mb-6">Shifts</h2>

      {shifts.length === 0 ? (
        <p className="text-xl text-gray-600 text-center py-8">No shifts found.</p>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {shifts.map((shift) => (
            <div
              key={shift.id}
              className="bg-white rounded-lg border-2 border-gray-300 hover:border-blue-500 hover:shadow-lg transition-all p-4"
            >
              {/* Header with Status Badge */}
              <div className="flex justify-between items-start mb-3">
                <div>
                  <p className="text-sm text-gray-600">
                    {shift.staff?.name || 'Unknown Staff'}
                  </p>
                  <p className="font-bold text-gray-900">
                    {new Date(shift.startTime).toLocaleDateString()}
                  </p>
                </div>
                <span
                  className={`px-3 py-1 rounded-full text-sm font-semibold ${
                    shift.status === 'Scheduled'
                      ? 'bg-blue-100 text-blue-800'
                      : shift.status === 'InProgress'
                      ? 'bg-green-100 text-green-800'
                      : 'bg-gray-100 text-gray-800'
                  }`}
                >
                  {shift.status}
                </span>
              </div>

              {/* Time */}
              <div className="bg-gray-50 rounded-lg p-3 mb-3">
                <p className="text-lg font-bold text-gray-900">
                  {new Date(shift.startTime).toLocaleTimeString('en-US', {
                    hour: '2-digit',
                    minute: '2-digit',
                  })}{' '}
                  →{' '}
                  {shift.endTime
                    ? new Date(shift.endTime).toLocaleTimeString('en-US', {
                        hour: '2-digit',
                        minute: '2-digit',
                      })
                    : 'N/A'}
                </p>
              </div>

              {/* Areas */}
              {shift.areaIds && shift.areaIds.length > 0 && (
                <div className="mb-3">
                  <p className="text-sm text-gray-600 font-semibold mb-1">Areas:</p>
                  <div className="flex flex-wrap gap-2">
                    {shift.areaIds.map((areaId) => (
                      <span
                        key={areaId}
                        className="px-2 py-1 bg-blue-100 text-blue-800 rounded text-sm"
                      >
                        {areaId}
                      </span>
                    ))}
                  </div>
                </div>
              )}

              {/* Action Buttons */}
              <div className="flex gap-2 pt-3 border-t">
                <button
                  onClick={() => setEditingShiftId(shift.id)}
                  className="flex-1 px-3 py-2 bg-blue-500 hover:bg-blue-600 text-white font-semibold rounded-lg text-sm transition-all"
                >
                  ✏️ Edit
                </button>
                <button
                  className="flex-1 px-3 py-2 bg-red-500 hover:bg-red-600 text-white font-semibold rounded-lg text-sm transition-all"
                  onClick={() => alert('Delete shift functionality coming soon')}
                >
                  🗑️ Delete
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};
