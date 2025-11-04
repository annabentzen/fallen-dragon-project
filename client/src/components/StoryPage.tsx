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

  const navigate = useNavigate();

  // Restart button handler
  const handleRestart = () => {
    console.log("Restart clicked, navigating to home");
    navigate("/");
  };

  // Load session info
  useEffect(() => {
    const loadSession = async () => {
      console.log("Loading session", sessionId);
      try {
        const sessionData = await getSession(sessionId);
        console.log("Loaded session:", sessionData);
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
    console.log("Loading current act for session", sessionId);
    setLoading(true);
    setErrorMsg(null);
    try {
      const result = await getCurrentAct(sessionId);
      console.log("getCurrentAct result:", result);

      if (!result || !result.act) {
        setErrorMsg(`No act found for session ${sessionId}.`);
        setCurrentAct(null);
        setChoices([]);
        return;
      }

      const { session, act } = result;

      console.log("Current act:", act);
      console.log("Act choices:", act.choices);

      setCurrentAct(act);
      setChoices(act.choices || []);
      setStoryEnded(session?.isCompleted ?? false);

      if (session) {
        console.log("Updating player session state:", session);
        setPlayerSession(session);
        setCharacterDesign(session.characterDesign || {});
      }
    } catch (error) {
      console.error("Error loading act:", error);
      setErrorMsg("Failed to load act.");
    } finally {
      setLoading(false);
      console.log("Finished loading act");
    }
  };

  useEffect(() => {
    loadAct();
  }, [sessionId]);

  // Handle choice selection
  const handleChoiceClick = async (nextActNumber: number) => {
    console.log("Choice clicked, nextActNumber:", nextActNumber);
    setLoading(true);
    try {
      await axios.post(`http://localhost:5151/api/story/nextAct/${sessionId}`, nextActNumber);
      console.log("Advanced to next act, reloading act");
      await loadAct();
    } catch (error) {
      console.error('Error advancing act:', error);
      setErrorMsg("Failed to advance to next act.");
    } finally {
      setLoading(false);
    }
  };

  // Loading & error states
  if (loading) {
    console.log("Rendering loading state");
    return <div>Loading...</div>;
  }

  if (errorMsg) {
    console.log("Rendering error state:", errorMsg);
    return <div className="error">{errorMsg}</div>;
  }

  if (!currentAct) {
    console.log("No act data available to render");
    return <div>No act data available.</div>;
  }

  console.log("Rendering story page for act", currentAct.actNumber);

  return (
    <div
      className="story-container"
      style={{
        backgroundColor: storyEnded ? "#7c372fff" : "#f0f8ff",
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
            {choices.length > 0 ? (
              choices.map((choice) => (
                <button
                  key={choice.choiceId}
                  onClick={() => handleChoiceClick(choice.nextActNumber)}
                  style={{
                    margin: '5px',
                    padding: '10px 20px',
                    backgroundColor: '#007bff',
                    color: 'white',
                    border: 'none',
                    borderRadius: '5px',
                    cursor: 'pointer',
                  }}
                >
                  {choice.text}
                </button>
              ))
            ) : (
              <p>No choices available.</p>
            )}
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
