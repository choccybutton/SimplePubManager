import React, { useState, useEffect } from 'react';
import DatePicker from 'react-datepicker';
import { Shift } from '../../types/api';
import 'react-datepicker/dist/react-datepicker.css';

interface ShiftFormProps {
  initialShift?: Shift;
  staffMembers: any[];
  areas: any[];
  onSubmit: (shiftData: any) => Promise<void>;
  onCancel: () => void;
  isLoading?: boolean;
}

export const ShiftForm: React.FC<ShiftFormProps> = ({
  initialShift,
  staffMembers,
  areas,
  onSubmit,
  onCancel,
  isLoading = false,
}) => {
  const [formData, setFormData] = useState({
    staffId: initialShift?.staffId || '',
    startTime: initialShift?.startTime ? new Date(initialShift.startTime) : new Date(),
    endTime: initialShift?.endTime ? new Date(initialShift.endTime) : new Date(),
    selectedAreas: initialShift?.areaIds || [],
  });

  const [errors, setErrors] = useState<Record<string, string>>({});

  const handleDateChange = (field: 'startTime' | 'endTime', date: Date | null) => {
    if (date) {
      setFormData(prev => ({ ...prev, [field]: date }));
      setErrors(prev => {
        const newErrors = { ...prev };
        delete newErrors[field];
        return newErrors;
      });
    }
  };

  const handleTimeChange = (field: 'startTime' | 'endTime', hours: number, minutes: number) => {
    const date = new Date(formData[field]);
    date.setHours(hours, minutes);
    handleDateChange(field, date);
  };

  const toggleArea = (areaId: string) => {
    setFormData(prev => ({
      ...prev,
      selectedAreas: prev.selectedAreas.includes(areaId)
        ? prev.selectedAreas.filter(id => id !== areaId)
        : [...prev.selectedAreas, areaId],
    }));
  };

  const validateForm = () => {
    const newErrors: Record<string, string> = {};

    if (!formData.staffId) newErrors.staffId = 'Staff member is required';
    if (!formData.startTime) newErrors.startTime = 'Start time is required';
    if (!formData.endTime) newErrors.endTime = 'End time is required';
    if (formData.endTime <= formData.startTime) {
      newErrors.endTime = 'End time must be after start time';
    }
    if (formData.selectedAreas.length === 0) newErrors.areas = 'At least one area must be selected';

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validateForm()) return;

    await onSubmit({
      staffId: formData.staffId,
      startTime: formData.startTime.toISOString(),
      endTime: formData.endTime.toISOString(),
      areaIds: formData.selectedAreas,
    });
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-6 p-6 bg-white rounded-lg shadow-lg">
      {/* Title */}
      <div>
        <h2 className="text-3xl font-bold text-gray-900 mb-2">
          {initialShift ? 'Edit Shift' : 'Create New Shift'}
        </h2>
        <p className="text-gray-600">Fill in the details below</p>
      </div>

      {/* Staff Selection */}
      <div>
        <label className="block text-xl font-semibold text-gray-900 mb-3">
          👤 Select Staff Member
        </label>
        <select
          value={formData.staffId}
          onChange={(e) => setFormData(prev => ({ ...prev, staffId: e.target.value }))}
          className="w-full px-4 py-3 text-lg border-2 border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
        >
          <option value="">-- Choose a staff member --</option>
          {staffMembers.map(staff => (
            <option key={staff.id} value={staff.id}>
              {staff.name} ({staff.email})
            </option>
          ))}
        </select>
        {errors.staffId && <p className="text-red-600 font-semibold mt-2">{errors.staffId}</p>}
      </div>

      {/* Date & Time Selection */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        {/* Start Date & Time */}
        <div>
          <label className="block text-xl font-semibold text-gray-900 mb-3">
            🕐 Start Date & Time
          </label>
          <div className="space-y-3">
            <DatePicker
              selected={formData.startTime}
              onChange={(date) => handleDateChange('startTime', date)}
              dateFormat="MMMM d, yyyy"
              className="w-full px-4 py-3 text-lg border-2 border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
              minDate={new Date()}
            />
            <div className="flex gap-2">
              <input
                type="number"
                min="0"
                max="23"
                value={formData.startTime.getHours()}
                onChange={(e) => handleTimeChange('startTime', parseInt(e.target.value), formData.startTime.getMinutes())}
                className="w-20 px-3 py-3 text-lg border-2 border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                placeholder="HH"
              />
              <span className="text-3xl font-bold text-gray-500">:</span>
              <input
                type="number"
                min="0"
                max="59"
                value={String(formData.startTime.getMinutes()).padStart(2, '0')}
                onChange={(e) => handleTimeChange('startTime', formData.startTime.getHours(), parseInt(e.target.value))}
                className="w-20 px-3 py-3 text-lg border-2 border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                placeholder="MM"
              />
            </div>
            {errors.startTime && <p className="text-red-600 font-semibold">{errors.startTime}</p>}
          </div>
        </div>

        {/* End Date & Time */}
        <div>
          <label className="block text-xl font-semibold text-gray-900 mb-3">
            🕐 End Date & Time
          </label>
          <div className="space-y-3">
            <DatePicker
              selected={formData.endTime}
              onChange={(date) => handleDateChange('endTime', date)}
              dateFormat="MMMM d, yyyy"
              className="w-full px-4 py-3 text-lg border-2 border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
              minDate={formData.startTime}
            />
            <div className="flex gap-2">
              <input
                type="number"
                min="0"
                max="23"
                value={formData.endTime.getHours()}
                onChange={(e) => handleTimeChange('endTime', parseInt(e.target.value), formData.endTime.getMinutes())}
                className="w-20 px-3 py-3 text-lg border-2 border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                placeholder="HH"
              />
              <span className="text-3xl font-bold text-gray-500">:</span>
              <input
                type="number"
                min="0"
                max="59"
                value={String(formData.endTime.getMinutes()).padStart(2, '0')}
                onChange={(e) => handleTimeChange('endTime', formData.endTime.getHours(), parseInt(e.target.value))}
                className="w-20 px-3 py-3 text-lg border-2 border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                placeholder="MM"
              />
            </div>
            {errors.endTime && <p className="text-red-600 font-semibold">{errors.endTime}</p>}
          </div>
        </div>
      </div>

      {/* Areas Selection */}
      <div>
        <label className="block text-xl font-semibold text-gray-900 mb-3">
          📍 Select Work Areas
        </label>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-3">
          {areas.map(area => (
            <button
              key={area.id}
              type="button"
              onClick={() => toggleArea(area.id)}
              className={`p-4 rounded-lg border-2 font-semibold text-lg transition-all ${
                formData.selectedAreas.includes(area.id)
                  ? 'bg-blue-500 border-blue-600 text-white shadow-lg'
                  : 'bg-white border-gray-300 text-gray-900 hover:border-blue-300'
              }`}
            >
              {area.name}
              {formData.selectedAreas.includes(area.id) && ' ✓'}
            </button>
          ))}
        </div>
        {errors.areas && <p className="text-red-600 font-semibold mt-2">{errors.areas}</p>}
      </div>

      {/* Action Buttons */}
      <div className="flex gap-4 pt-6 border-t-2">
        <button
          type="submit"
          disabled={isLoading}
          className="flex-1 px-6 py-4 bg-green-500 text-white font-bold text-lg rounded-lg hover:bg-green-600 disabled:bg-gray-400 shadow-lg"
        >
          {isLoading ? '⏳ Saving...' : '✓ Save Shift'}
        </button>
        <button
          type="button"
          onClick={onCancel}
          className="flex-1 px-6 py-4 bg-gray-500 text-white font-bold text-lg rounded-lg hover:bg-gray-600 shadow-lg"
        >
          ✕ Cancel
        </button>
      </div>
    </form>
  );
};
