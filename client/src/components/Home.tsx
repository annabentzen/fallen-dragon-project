// src/components/Home.tsx
import { useNavigate } from 'react-router-dom';
import { startStory } from '../services/storyApi'; // Import the API call to backend

export default function Home() {
  const navigate = useNavigate();

  const handleStart = async () => {
    try {
      // Ask backend to start a new story session
      // This returns something like: { sessionId: "b21d4f2e-8c1e-43e8-a7d8-..." }
      const data = await startStory();

      // Navigate to the story route with the backend-provided sessionId
      navigate(`/story/${data.sessionId}`);
    } catch (error) {
      console.error('Failed to start story session:', error);
      alert('Could not start story. Please try again.');
    }
  };

  return (
    <div className="home">
      <h1>Fallen Dragon</h1>
      <p>Begin your journey and shape your destiny.</p>
      <button onClick={handleStart}>Start Story</button>
    </div>
  );
}

/*
Explanation:

When the Start Story button is clicked:

The frontend calls POST /api/story/start (implemented in your C# backend).

The backend creates and returns a unique sessionId (Guid.NewGuid()).

React navigates to /story/{sessionId} to load the first act.

This mimics a real app that creates and tracks player sessions on the server.
*/