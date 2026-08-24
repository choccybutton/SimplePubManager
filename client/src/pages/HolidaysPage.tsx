import React from 'react';
import { HolidayList } from '../components/Holidays/HolidayList';
import { MainLayout } from '../components/Layout/MainLayout';

export const HolidaysPage: React.FC = () => {
  return (
    <MainLayout>
      <HolidayList />
    </MainLayout>
  );
};
