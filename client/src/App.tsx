import React from 'react';
import { BrowserRouter as Router, Routes, Route, useNavigate } from 'react-router-dom';
import Home from './pages/Home';
import StoryPage from './components/StoryPage';
import EndingScreen from './components/EndingScreen';

const App: React.FC = () => {
  return (
    <Router>
      <Routes>
        {/* Home / character builder */}
        <Route path="/" element={<Home />} />

        {/* Story page with dynamic sessionId */}
        <Route path="/story/:sessionId" element={<StoryPageWrapper />} />

        {/* Optional direct ending screen */}
        <Route path="/ending" element={<EndingScreenWrapper />} />
      </Routes>
    </Router>
  );
};

// Wrapper for Endingscreen to handle navigation
const EndingScreenWrapper: React.FC = () => {
  const navigate = useNavigate();

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
