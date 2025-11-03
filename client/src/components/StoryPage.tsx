// src/components/StoryPage.tsx
import React, { useEffect, useState } from 'react';
import EndingScreen from './EndingScreen';
import { getCurrentAct, getSession } from '../services/storyApi';
import { Act, Choice, PlayerSessionFromApi, CharacterDesign } from '../types/story';
import axios from 'axios';
import { useNavigate } from "react-router-dom";

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
  const [errorMsg, setErrorMsg] = useState<string | null>(null);

  const navigate = useNavigate(); // For navigation to Home

  // Restart button handler
  const handleRestart = () => {
    navigate("/"); // Redirect to home page
  };

  // Load session info
  useEffect(() => {
    const loadSession = async () => {
      try {
        const sessionData = await getSession(sessionId);
        if (!sessionData) {
          setErrorMsg(`Session ${sessionId} not found.`);
          return;
        }
        setPlayerSession(sessionData);
        setCharacterDesign(sessionData.characterDesign || {});
      } catch (error) {
        console.error("Error loading session:", error);
        setErrorMsg("Failed to load session data.");
      }
    };
    loadSession();
  }, [sessionId]);

  // Load current act
  const loadAct = async () => {
    setLoading(true);
    setErrorMsg(null);
    try {
      const result = await getCurrentAct(sessionId); // Returns { session, act }
      if (!result || !result.act) {
        setErrorMsg(`No act found for session ${sessionId}.`);
        setCurrentAct(null);
        return;
      }

      const { session, act } = result;

      setCurrentAct(act);
      setChoices(Array.isArray(act.choices) ? act.choices : []);
      setStoryEnded(session?.isCompleted ?? false); // If completed, story ended

      if (session) {
        setPlayerSession(session);
        setCharacterDesign(session.characterDesign);
      }
    } catch (error) {
      console.error("Error loading act:", error);
      setErrorMsg("Failed to load act.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadAct();
  }, [sessionId]);

  // Handle choice selection
  const handleChoiceClick = async (nextActNumber: number) => {
    setLoading(true);
    try {
      await axios.post(`http://localhost:5151/api/story/nextAct/${sessionId}`, nextActNumber); // Match backend route
      await loadAct(); // Reload after advancing act
    } catch (error) {
      console.error('Error advancing act:', error);
    } finally {
      setLoading(false);
    }
  };

  // Loading & error states
  if (loading) return <div>Loading...</div>;
  if (errorMsg) return <div className="error">{errorMsg}</div>;
  if (!currentAct) return <div>No act data available.</div>;

  // Story vs Ending background color
  return (
    <div
      className="story-container"
      style={{
        backgroundColor: storyEnded ? "#7c372fff" : "#f0f8ff", // Ending page vs Story page
        minHeight: "100vh",
        padding: "20px",
      }}
    >
      {storyEnded ? (
        <EndingScreen onRestart={handleRestart} />
      ) : (
        <>
          <h2>Act {currentAct.actNumber}</h2>
          <p>{currentAct.text}</p>

          <div className="choices">
            {choices.map((choice) => (
              <button
                key={choice.id}
                onClick={() => handleChoiceClick(choice.nextActNumber)}
                style={{ margin: "5px" }}
              >
                {choice.text}
              </button>
            ))}
          </div>

          {playerSession && (
            <div className="character-corner" style={{ marginTop: "20px" }}>
              <p>{playerSession.characterName}</p>
              <div
                className="character-visual"
                style={{
                  backgroundColor: characterDesign.color || "blue",
                  width: "50px",
                  height: "50px",
                  borderRadius: "50%",
                }}
              />
            </div>
          )}
        </>
      )}
    </div>
  );
};

export default StoryPage;
