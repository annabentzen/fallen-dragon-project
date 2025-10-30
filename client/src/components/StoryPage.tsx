// src/components/StoryPage.tsx
import React, { useEffect, useState } from 'react';
import EndingScreen from './EndingScreen';
import { useParams } from 'react-router-dom';
import { getAct, getChoicesForAct, getCurrentActForSession, updateSessionProgress } from '../services/storyApi';


// TypeScript interfaces matching your backend models
interface Choice {
  choiceId: number;
  actId: number;
  text: string;
  nextActNumber: number;
}

interface Act {
  id: number;
  storyId: number;
  actNumber: number;
  text: string;
  choices: Choice[];
}

const StoryPage: React.FC = () => {
  // Get sessionId from URL (e.g. /story/1234 or /story/b21d4f2e-8c1e...)
  const { sessionId } = useParams<{ sessionId: string }>();

  // Local component state
  const [currentAct, setCurrentAct] = useState<Act | null>(null);
  const [choices, setChoices] = useState<Choice[]>([]);
  const [loading, setLoading] = useState(true);
  const [storyEnded, setStoryEnded] = useState(false);

  /**
   * Loads a specific act and its choices from the backend.
   * This fetches both the act text and the available choices.
   */
  const loadAct = async (actNumber: number) => {
    setLoading(true);
    try {
      // Fetch act content (text, act number, etc.)
      const actData: Act = await getAct(actNumber);
      setCurrentAct(actData);

      // Fetch all choices for that act
      const actChoices: Choice[] = await getChoicesForAct(actData.id);
      setChoices(actChoices);

      // If there are no more choices, this act ends the story
      setStoryEnded(actChoices.length === 0);
    } catch (error) {
      console.error('Error loading act:', error);
    } finally {
      setLoading(false);
    }
  };

  // When component first mounts, start from Act 1
  useEffect(() => {
    loadAct(1);
  }, [sessionId]); // Re-run if sessionId changes (new story session)

  /**
   * When a choice is clicked, move to the act it leads to.
   */
  const handleChoiceClick = (nextActNumber: number) => {
    loadAct(nextActNumber);
  };

  // Show loading state while data is being fetched
  if (loading) return <div>Loading...</div>;

  // If something went wrong fetching the act
  if (!currentAct) return <div>Act not found.</div>;

  // If the story has no more choices, show the ending screen
  if (storyEnded) return <EndingScreen />;

  useEffect(() => {
  if (sessionId) {
    loadSessionAct(); // fetch the current act for that session
  }
}, [sessionId]);

const loadSessionAct = async () => {
  try {
    const actData: Act = await getCurrentActForSession(sessionId!);
    setCurrentAct(actData);
    const actChoices: Choice[] = await getChoicesForAct(actData.id);
    setChoices(actChoices);
  } catch (error) {
    console.error('Error loading act for session:', error);
  } finally {
    setLoading(false);
  }
};

const handleChoiceClick = async (nextActNumber: number) => {
  try {
    await updateSessionProgress(sessionId!, nextActNumber);
    await loadSessionAct();
  } catch (error) {
    console.error('Error updating session progress:', error);
  }
};


  // Otherwise, show the current act and all its choices as buttons
  return (
    <div className="story-container">
      <h2>Act {currentAct.actNumber}</h2>
      <p>{currentAct.text}</p>

      <div className="choices">
        {choices.map((choice) => (
          <button
            key={choice.choiceId}
            onClick={() => handleChoiceClick(choice.nextActNumber)}
          >
            {choice.text}
          </button>
        ))}
      </div>
    </div>
  );
};

export default StoryPage;
