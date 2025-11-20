import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

interface ProtectedRouteProps {
  children: React.ReactElement;
}

/**
 * Wrapper for routes that require authentication
 * Redirects to /login if user is not authenticated
 */
const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children }) => {
  const { isAuthenticated } = useAuth();

  if (!isAuthenticated) {
    // User is not authenticated - redirect to login
    return <Navigate to="/login" replace />;
  }

  // User is authenticated - show the content
  return children;
};

export default ProtectedRoute;