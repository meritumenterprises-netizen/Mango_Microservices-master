import type { ReactNode } from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '../state/AuthContext';

export function RequireLogin({ children }: { children: ReactNode }) {
  const { isLoggedIn } = useAuth();
  const location = useLocation();
  return isLoggedIn ? <>{children}</> : <Navigate to="/login" replace state={{ from: location.pathname }} />;
}
