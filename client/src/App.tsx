import React, { JSX, useEffect } from 'react';
import { AuthProvider } from './contexts/AuthContext';
import { BrowserRouter as Router, Routes, Route, useNavigate, Navigate, useParams } from 'react-router-dom';
import Home from './components/Home';
import StoryPage from './components/StoryPage';
import EndingScreen from './components/EndingScreen';
import LoginPage from "./components/LogInPage";
import RegisterPage from "./components/RegisterPage";
import { isAuthenticated } from "./services/authApi";

// Protected Route Component - only allows access if user is logged in
const ProtectedRoute = ({ children }: { children: JSX.Element }) => {
  return isAuthenticated() ? children : <Navigate to="/" />;
};

const App: React.FC = () => {
  return (
    <AuthProvider>
    <Router>
      <Routes>
        {/* Public routes - Login is now the landing page */}
        <Route path="/" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />

        {/* Protected routes - require authentication */}
        <Route 
          path="/home" 
          element={
            <ProtectedRoute>
              <Home />
            </ProtectedRoute>
          } 
        />

        <Route 
          path="/story/:sessionId" 
          element={
            <ProtectedRoute>
              <StoryPageWrapper />
            </ProtectedRoute>
          } 
        />

        <Route 
          path="/ending/:sessionId" 
          element={
            <ProtectedRoute>
              <EndingScreenWrapper />
            </ProtectedRoute>
          } 
        />
      </Routes>
    </Router>
    </AuthProvider>
  );
};

// Wrapper for EndingScreen to handle navigation
const EndingScreenWrapper: React.FC = () => {
  const navigate = useNavigate();
  const { sessionId } = useParams<{ sessionId: string }>();

  const handleRestart = () => {
    navigate('/home'); // Navigate back to home to start a new session
  };

  return <EndingScreen onRestart={handleRestart} endingType={'default'} endingText={''} />;
};

// Wrapper to pass sessionId from URL to StoryPage
const StoryPageWrapper: React.FC = () => {
  const { sessionId } = useParams<{ sessionId: string }>();

  if (!sessionId) return <div>No session ID provided.</div>;

  const parsedId = parseInt(sessionId, 10);
  if (isNaN(parsedId)) return <div>Invalid session ID.</div>;

  return <StoryPage sessionId={parsedId} />;
};

export default App;