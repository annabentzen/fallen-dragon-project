import React from 'react';
import { BrowserRouter as Router, Routes, Route, useNavigate } from 'react-router-dom';
import Home from './components/Home';
import StoryPage from './components/StoryPage';
import EndingScreen from './components/EndingScreen';
import { AuthProvider } from './contexts/AuthContext';
import ProtectedRoute from './components/ProtectedRoute';
import LoginPage from './components/LoginPage';

const App: React.FC = () => {
  return (
    <AuthProvider>
    <Router>
      <Routes>
        {/* Login route - PUBLIC */}
        <Route path="/login" element={<LoginPage />} />

        {/* Home / character builder - PROTECTED */}
        <Route path="/" element={<ProtectedRoute><Home /></ProtectedRoute>} />

        {/* Story page with dynamic sessionId - PROTECTED */}
        <Route path="/story/:sessionId" element={<ProtectedRoute><StoryPageWrapper /></ProtectedRoute>} />
        {/* Optional direct ending screen - PROTECTED */}
        <Route path="/ending/:sessionId" element={<ProtectedRoute><EndingScreenWrapper /></ProtectedRoute>} />
      </Routes>
    </Router>
    </AuthProvider>
  );
};

// Wrapper for Endingscreen to handle navigation
const EndingScreenWrapper: React.FC = () => {
  const navigate = useNavigate();
  const { sessionId } = useParams<{ sessionId: string }>();

  const handleRestart = () => {
    navigate('/'); // Navigate back to home to start a new session
  };

  return <EndingScreen onRestart={handleRestart} />;
};

// Wrapper to pass sessionId from URL to StoryPage
import { useParams } from 'react-router-dom';
const StoryPageWrapper: React.FC = () => {
  const { sessionId } = useParams<{ sessionId: string }>();

  if (!sessionId) return <div>No session ID provided.</div>;

  const parsedId = parseInt(sessionId, 10);
  if (isNaN(parsedId)) return <div>Invalid session ID.</div>;

  return <StoryPage sessionId={parsedId} />;
};


export default App;
