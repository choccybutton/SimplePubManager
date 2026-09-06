import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import { DeviceProvider } from './context/DeviceContext';
import { ErrorBoundary } from './components/Common/ErrorBoundary';
import { ProtectedRoute } from './components/Auth/ProtectedRoute';
import { UserRole } from './types';
import './styles/datepicker.css';

// Pages
import { LoginPage } from './pages/LoginPage';
import { DashboardPage } from './pages/DashboardPage';
import { ShiftsPage } from './pages/ShiftsPage';
import { TasksPage } from './pages/TasksPage';
import { HolidaysPage } from './pages/HolidaysPage';
import { PaymentsPage } from './pages/PaymentsPage';
import { StaffPage } from './pages/StaffPage';
import { AreasPage } from './pages/AreasPage';
import { SharedDevicePage } from './pages/SharedDevicePage';

export const App: React.FC = () => {
  return (
    <ErrorBoundary>
      <Router>
        <AuthProvider>
          <DeviceProvider>
            <Routes>
              <Route path="/login" element={<LoginPage />} />
              <Route path="/shared-device" element={<SharedDevicePage />} />

              <Route
                path="/dashboard"
                element={
                  <ProtectedRoute>
                    <DashboardPage />
                  </ProtectedRoute>
                }
              />

              <Route
                path="/shifts"
                element={
                  <ProtectedRoute>
                    <ShiftsPage />
                  </ProtectedRoute>
                }
              />

              <Route
                path="/tasks"
                element={
                  <ProtectedRoute>
                    <TasksPage />
                  </ProtectedRoute>
                }
              />

              <Route
                path="/holidays"
                element={
                  <ProtectedRoute>
                    <HolidaysPage />
                  </ProtectedRoute>
                }
              />

              <Route
                path="/payments"
                element={
                  <ProtectedRoute allowedRoles={[UserRole.Manager, UserRole.Supervisor]}>
                    <PaymentsPage />
                  </ProtectedRoute>
                }
              />

              <Route
                path="/staff"
                element={
                  <ProtectedRoute allowedRoles={[UserRole.Manager, UserRole.Supervisor]}>
                    <StaffPage />
                  </ProtectedRoute>
                }
              />

              <Route
                path="/areas"
                element={
                  <ProtectedRoute allowedRoles={[UserRole.Manager]}>
                    <AreasPage />
                  </ProtectedRoute>
                }
              />

              <Route path="/" element={<Navigate to="/dashboard" />} />
              <Route path="*" element={<Navigate to="/dashboard" />} />
            </Routes>
          </DeviceProvider>
        </AuthProvider>
      </Router>
    </ErrorBoundary>
  );
};
