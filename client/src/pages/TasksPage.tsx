import React from 'react';
import { TaskList } from '../components/Tasks/TaskList';
import { MainLayout } from '../components/Layout/MainLayout';

export const TasksPage: React.FC = () => {
  return (
    <MainLayout>
      <TaskList />
    </MainLayout>
  );
};
