import React from 'react';
import { ShiftList } from '../components/Shifts/ShiftList';
import { MainLayout } from '../components/Layout/MainLayout';

export const ShiftsPage: React.FC = () => {
  return (
    <MainLayout>
      <ShiftList />
    </MainLayout>
  );
};
