import React from 'react';
import { UserRole } from '../../types';

interface RoleBasedViewProps {
  allowedRoles: UserRole[];
  userRole: UserRole | null;
  children: React.ReactNode;
  fallback?: React.ReactNode;
}

export const RoleBasedView: React.FC<RoleBasedViewProps> = ({
  allowedRoles,
  userRole,
  children,
  fallback = <p>You do not have permission to view this content.</p>,
}) => {
  if (!userRole || !allowedRoles.includes(userRole)) {
    return <>{fallback}</>;
  }

  return <>{children}</>;
};
