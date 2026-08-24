import React from 'react';
import { Dashboard } from '../components/Common/Dashboard';
import { MainLayout } from '../components/Layout/MainLayout';

export const DashboardPage: React.FC = () => {
  return (
    <MainLayout>
      <Dashboard />
    </MainLayout>
  );
};
