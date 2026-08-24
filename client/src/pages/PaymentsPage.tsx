import React from 'react';
import { PaymentList } from '../components/Payments/PaymentList';
import { MainLayout } from '../components/Layout/MainLayout';

export const PaymentsPage: React.FC = () => {
  return (
    <MainLayout>
      <PaymentList />
    </MainLayout>
  );
};
