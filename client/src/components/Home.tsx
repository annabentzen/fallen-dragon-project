import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { createSession } from '../services/storyApi';

export default function Home() {
  const navigate = useNavigate();
  const [characterName, setCharacterName] = useState('');
  const [characterDesign, setCharacterDesign] = useState({ hair: 'brown', outfit: 'armor', color: 'blue' });

  const startStory = async () => {
    const session = await createSession({ characterName, characterDesign, storyId: 1 });
    navigate(`/story/${session.sessionId}`);
  };

  return (
    <div className="home">
      <h1>Fallen Dragon</h1>
      <div className="character-builder">
        <input
          placeholder="Enter character name"
          value={characterName}
          onChange={e => setCharacterName(e.target.value)}
        />
        <button onClick={() => setCharacterDesign({ ...characterDesign, hair: 'black' })}>Black Hair</button>
        <button onClick={() => setCharacterDesign({ ...characterDesign, outfit: 'robe' })}>Robe</button>
        <button onClick={() => setCharacterDesign({ ...characterDesign, color: 'red' })}>Red Color</button>
      </div>
      <button onClick={startStory}>Start Story</button>
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