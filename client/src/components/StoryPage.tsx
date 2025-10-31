// src/components/StoryPage.tsx
import React, { useEffect, useState } from 'react';
import EndingScreen from './EndingScreen';
import { getCurrentAct, getSession } from '../services/storyApi';
import { Act, Choice, PlayerSessionFromApi, CharacterDesign } from '../types/story';

interface StoryPageProps {
  sessionId: number;
}

const StoryPage: React.FC<StoryPageProps> = ({ sessionId }) => {
  const [currentAct, setCurrentAct] = useState<Act | null>(null);
  const [choices, setChoices] = useState<Choice[]>([]);
  const [loading, setLoading] = useState(true);
  const [storyEnded, setStoryEnded] = useState(false);
  const [playerSession, setPlayerSession] = useState<PlayerSessionFromApi | null>(null);
  const [characterDesign, setCharacterDesign] = useState<CharacterDesign>({});

  // Load session info
  useEffect(() => {
  const loadSession = async () => {
    try {
      const sessionData = await getSession(sessionId);
      setPlayerSession(sessionData);
      setCharacterDesign(sessionData.characterDesign); // now camelCase, not characterDesignJson
    } catch (error) {
      console.error("Error loading session:", error);
    }
  };
  loadSession();
}, [sessionId]);



  // Load current act
  const loadAct = async () => {
    setLoading(true);
    try {
      const data = await getCurrentAct(sessionId);
      setCurrentAct(data.act);
      setChoices(data.act.choices || []);
      setStoryEnded(data.act.choices.length === 0);
    } catch (error) {
      console.error('Error loading act:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadAct();
  }, [sessionId]);

  const handleChoiceClick = async (nextActNumber: number) => {
    setLoading(true);
    try {
      // Optionally call backend to update session's current act here
      await loadAct();
    } catch (error) {
      console.error('Error advancing act:', error);
    } finally {
      setLoading(false);
    }
  };

  if (loading) return <div>Loading...</div>;
  if (!currentAct) return <div>Act not found.</div>;
  if (storyEnded) return <EndingScreen />;

  return (
    <div className="story-container">
      <h2>Act {currentAct.actNumber}</h2>
      <p>{currentAct.text}</p>
      <div className="choices">
        {choices.map((choice) => (
          <button key={choice.id} onClick={() => handleChoiceClick(choice.nextActNumber)}>
            {choice.text}
          </button>
        ))}
      </div>

      {playerSession && (
        <div className="character-corner">
          <p>{playerSession.characterName}</p>
          <div
            className="character-visual"
            style={{
              backgroundColor: characterDesign.color || 'blue',
              // optionally display hair/outfit via images
            }}
          />
        </div>
      )}
    </div>
  );
};

export default StoryPage;
