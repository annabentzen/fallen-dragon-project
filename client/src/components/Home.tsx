import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { createSession, safeParseCharacterDesign } from '../services/storyApi';
import {
  getAllPoses,
  CharacterPose,
  saveCharacterToLocal,
  clearSavedCharacter,
  loadCharacterFromLocal,
} from '../services/characterApi';
import CharacterBuilder from './CharacterBuilder';

export default function Home() {
  const navigate = useNavigate();

  // Character state
  const [characterName, setCharacterName] = useState('');
  const [hair, setHair] = useState('hair1.png');
  const [face, setFace] = useState('face1.png');
  const [outfit, setOutfit] = useState('clothing1.png');
  const [poseId, setPoseId] = useState<number | null>(null);

  // UI state
  const [poses, setPoses] = useState<CharacterPose[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Load poses and last character on mount
  useEffect(() => {
    const fetchPoses = async () => {
      try {
        const fetchedPoses = await getAllPoses();
        console.log('Fetched poses for Home:', fetchedPoses);
        setPoses(fetchedPoses);
      } catch (err) {
        console.error('Failed to fetch poses for Home:', err);
      }
    };

    const loadLastCharacter = () => {
      const lastCharacter = loadCharacterFromLocal();
      console.log('Loaded last character from localStorage:', lastCharacter);

      if (lastCharacter) {
        const parsedDesign = safeParseCharacterDesign(lastCharacter);
        if (parsedDesign.hair) setHair(parsedDesign.hair);
        if (parsedDesign.face) setFace(parsedDesign.face);
        if (parsedDesign.outfit) setOutfit(parsedDesign.outfit);
        if (parsedDesign.poseId !== undefined) setPoseId(parsedDesign.poseId);
      }
    };

    fetchPoses();
    loadLastCharacter();
  }, []);

  // Reset character to defaults
  const resetCharacter = () => {
    console.log('Resetting character...');
    clearSavedCharacter();
    setCharacterName('');
    setHair('hair1.png');
    setFace('face1.png');
    setOutfit('clothing1.png');
    setPoseId(null);
    setError(null);
  };

  // Start the story
  const startStory = async () => {
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
      const character = { hair, face, outfit, poseId };
      saveCharacterToLocal(character);
      console.log('Character saved to localStorage:', character);

      const session = await createSession({
        characterName,
        characterDesign: character,
        storyId: 1,
      });
      console.log('Session created:', session);

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
      <h1 style={{ textAlign: 'center', color: '#333' }}>The Fallen Dragon</h1>

      <div
        style={{
          textAlign: 'center',
          margin: '20px 0',
          padding: '15px',
          backgroundColor: '#f5f5f5',
          borderRadius: '8px',
        }}
      >
        <p>
          A dragon has fallen and landed in your village. Hero, will you save it? Your choices will
          determine its fate.
        </p>
        <p>
          <strong>Create your hero and begin your journey...</strong>
        </p>
      </div>

      {error && (
        <div
          style={{
            backgroundColor: '#ffebee',
            color: '#c62828',
            padding: '10px',
            borderRadius: '4px',
            marginBottom: '15px',
          }}
        >
          {error}
        </div>
      )}

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
            borderRadius: '4px',
          }}
          disabled={loading}
        />
      </div>

      <CharacterBuilder
        hair={hair}
        face={face}
        outfit={outfit}
        poseId={poseId}
        poses={poses}
        onHairChange={setHair}
        onFaceChange={setFace}
        onOutfitChange={setOutfit}
        onPoseChange={setPoseId}
      />

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
          marginTop: '20px',
        }}
      >
        {loading ? 'Starting...' : 'Start Mission'}
      </button>

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
          marginTop: '10px',
        }}
      >
        Reset Character
      </button>
    </div>
  );
}
