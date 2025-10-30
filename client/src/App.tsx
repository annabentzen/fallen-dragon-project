import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Home from './components/Home';
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
        <Route path="/ending" element={<EndingScreen />} />
      </Routes>
    </Router>
  );
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
