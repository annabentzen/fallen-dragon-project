import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { createSession } from '../services/storyApi';
import {
  getAllPoses,
  CharacterPose,
  saveCharacterToLocal,
  clearSavedCharacter,
  loadCharacterFromLocal
} from '../services/characterApi';
import CharacterBuilder from './CharacterBuilder';

export default function Home() {
  const navigate = useNavigate();

// character state
  const [characterName, setCharacterName] = useState('');
  const [hair, setHair] = useState('hair1.png');
  const [face, setFace] = useState('face1.png');
  const [clothing, setClothing] = useState('clothing1.png');
  const [poseId, setPoseId] = useState<number | null>(null);

// UI state

const [poses, setPoses] = useState<CharacterPose[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);


// load poses and last character on mount
useEffect(() => {
    loadPoses();
    loadLastCharacter();
  }, []);

  const loadPoses = async () => {
    try {
      const fetchedPoses = await getAllPoses();
      setPoses(fetchedPoses);
    } catch (err) {
      console.error('Failed to load poses:', err);
      setError('Failed to load poses. Make sure your backend is running on port 5151.');
    }
  };

  const loadLastCharacter = () => {
    const lastCharacter = loadCharacterFromLocal();
    if (lastCharacter) {
      setHair(lastCharacter.hair);
      setFace(lastCharacter.face);
      setClothing(lastCharacter.clothing);
      setPoseId(lastCharacter.poseId);
    }
  };

  const resetCharacter = () => {
    // Clear localStorage
    clearSavedCharacter();
    
    // Reset to defaults
    setCharacterName('');
    setHair('hair1.png');
    setFace('face1.png');
    setClothing('clothing1.png');
    setPoseId(null);
    
    // Clear any errors
    setError(null);
  };

  const startStory = async () => {
    // Validation
    if (!characterName.trim()) {
      setError('Please enter a hero name');
      return;
    }

    if (!poseId) {
      setError('Please select a pose');
      return;
    }

    setLoading(true);
    setError(null);

    try {
      // Save character to localStorage for "Start Over"
      saveCharacterToLocal({ hair, face, clothing, poseId });

      // Create session with character design
      const session = await createSession({
        characterName,
        characterDesign: { hair, outfit: clothing, color: 'default' },
        storyId: 1,
      });

      // Navigate to story
      navigate(`/story/${session.sessionId}`);
    } catch (err) {
      console.error('Failed to start story:', err);
      setError('Failed to start story. Check if your backend is running.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ maxWidth: '800px', margin: '0 auto', padding: '20px' }}>
      {/* Title */}
      <h1 style={{ textAlign: 'center', color: '#333' }}>The Fallen Dragon</h1>

      {/* Intro Text */}
      <div style={{ 
        textAlign: 'center', 
        margin: '20px 0', 
        padding: '15px',
        backgroundColor: '#f5f5f5',
        borderRadius: '8px'
      }}>
        <p>
          A dragon has fallen and landed in your village. Hero,
          will you save it? Your choices will determine its fate.
        </p>
        <p><strong>Create your hero and begin your journey...</strong></p>
      </div>

      {/* Error Message */}
      {error && (
        <div style={{ 
          backgroundColor: '#ffebee', 
          color: '#c62828', 
          padding: '10px', 
          borderRadius: '4px',
          marginBottom: '15px'
        }}>
          {error}
        </div>
      )}

      {/* Hero Name Input */}
      <div style={{ marginBottom: '20px' }}>
        <label style={{ display: 'block', marginBottom: '5px', fontWeight: 'bold' }}>
          Hero Name:
        </label>
        <input
          type="text"
          placeholder="Enter your hero's name"
          value={characterName}
          onChange={(e) => setCharacterName(e.target.value)}
          style={{ 
            width: '100%', 
            padding: '10px', 
            fontSize: '16px',
            border: '1px solid #ccc',
            borderRadius: '4px'
          }}
          disabled={loading}
        />
      </div>

      {/* Character Builder */}
      <CharacterBuilder
        hair={hair}
        face={face}
        clothing={clothing}
        poseId={poseId}
        poses={poses}
        onHairChange={setHair}
        onFaceChange={setFace}
        onClothingChange={setClothing}
        onPoseChange={setPoseId}
      />

      {/* Start Button */}
      <button 
        onClick={startStory}
        disabled={loading}
        style={{
          width: '100%',
          padding: '15px',
          fontSize: '18px',
          fontWeight: 'bold',
          backgroundColor: loading ? '#ccc' : '#4CAF50',
          color: 'white',
          border: 'none',
          borderRadius: '8px',
          cursor: loading ? 'not-allowed' : 'pointer',
          marginTop: '20px'
        }}
      >
        {loading ? 'Starting...' : 'Start Mission'}
      </button>

      {/* Reset Character Button */}
      <button 
        onClick={resetCharacter}
        disabled={loading}
        style={{
          width: '100%',
          padding: '12px',
          fontSize: '16px',
          backgroundColor: 'transparent',
          color: '#666',
          border: '2px solid #ccc',
          borderRadius: '8px',
          cursor: loading ? 'not-allowed' : 'pointer',
          marginTop: '10px'
        }}
      >
        Reset Character
      </button>
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