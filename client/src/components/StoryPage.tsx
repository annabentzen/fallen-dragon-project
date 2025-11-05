// src/components/StoryPage.tsx
import React, { useEffect, useState } from 'react';
import EndingScreen from './EndingScreen';
import { getCurrentAct, getSession } from '../services/storyApi';
import { Act, Choice, PlayerSessionFromApi, CharacterDesign, CharacterPose } from '../types/story';
import axios from 'axios';
import { useNavigate } from "react-router-dom";
import { getAllPoses } from '../services/characterApi';

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
      // Wrap number in object so backend can bind it
      await axios.post(
        `http://localhost:5151/api/story/nextAct/${sessionId}`,
        { nextActNumber }, // must be an object
        { headers: { "Content-Type": "application/json" } }
      );
      console.log("POST successful, loading next act...");
      await loadAct(); // Reload after advancing act
    } catch (error) {
      console.error("Error advancing act:", error);
      setErrorMsg("Failed to advance to next act.");
    } finally {
      setLoading(false);
    }
  };

  // Add this at the top inside StoryPage component
const [poses, setPoses] = useState<CharacterPose[]>([]);

// Fetch poses on mount
useEffect(() => {
  const fetchPoses = async () => {
    try {
      const fetchedPoses = await getAllPoses();
      console.log("Fetched poses for StoryPage:", fetchedPoses);
      setPoses(fetchedPoses);
    } catch (err) {
      console.error("Failed to fetch poses:", err);
    }
  };
  fetchPoses();
}, []);


  useEffect(() => {
  const loadPoses = async () => {
    try {
      const res = await axios.get<CharacterPose[]>("http://localhost:5151/api/poses");
      setPoses(res.data);
      console.log("Loaded poses:", res.data);
    } catch (err) {
      console.error("Error loading poses:", err);
    }
  };
  loadPoses();
}, []);


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
  
  if (playerSession) {
  const selectedPose = poses.find(p => p.id === characterDesign.poseId);
  console.log("Rendering pose:", characterDesign.poseId, selectedPose, poses);
}

const selectedPose = poses.find(p => p.id === characterDesign.poseId);
console.log("Rendering pose:", characterDesign.poseId, selectedPose, poses);



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
                style={{
                  width: '100px',
                  height: '100px',
                  position: 'relative',
                  margin: '10px 0',
                }}
              >
                {/* Base image */}
                <img
                  src="/images/base.png"
                  alt="base"
                  style={{ position: 'absolute', width: '100%', height: '100%', objectFit: 'contain' }}
                />

                {/* Hair */}
                {characterDesign.hair && (
                  <img
                    src={`/images/hair/${characterDesign.hair}`}
                    alt="hair"
                    style={{ position: 'absolute', width: '100%', height: '100%', objectFit: 'contain' }}
                  />
                )}

                {/* Face */}
                {characterDesign.face && (
                  <img
                    src={`/images/faces/${characterDesign.face}`}
                    alt="face"
                    style={{ position: 'absolute', width: '100%', height: '100%', objectFit: 'contain' }}
                  />
                )}

                {/* Clothing / Outfit */}
                {characterDesign.outfit && (
                  <img
                    src={`/images/clothes/${characterDesign.outfit}`}
                    alt="clothing"
                    style={{ position: 'absolute', width: '100%', height: '100%', objectFit: 'contain' }}
                  />
                )}

                {/* Pose */}             
                 {characterDesign.poseId && poses.length > 0 && (
                    <img
                      src={`/images/poses/${poses.find(p => p.id === characterDesign.poseId)?.imageUrl}`}
                      alt="pose"
                      style={{ position: 'absolute', width: '100%', height: '100%', objectFit: 'contain' }}
                    />
                  )}
              </div>
            </div>
          )}

        </>
      )}
    </div>
  );
};

export default StoryPage;
