import React, { useEffect, useState } from "react";
import EndingScreen from "./EndingScreen";
import CharacterBuilder from "./CharacterBuilder";
import { useNavigate } from "react-router-dom";

import {
  getCharacterForSession,
  getCurrentAct,
  getSession,
  moveToNextAct,
  updateCharacter,
} from "../services/storyApi";
import { getAllPoses } from "../services/characterApi";
import { Act, Choice, PlayerSessionFromApi } from "../types/story";
import { Character, CharacterPose } from "../types/character";

interface StoryPageProps {
  sessionId: number;
}

const StoryPage: React.FC<StoryPageProps> = ({ sessionId }) => {
  const [currentAct, setCurrentAct] = useState<Act | null>(null);
  const [choices, setChoices] = useState<Choice[]>([]);
  const [loading, setLoading] = useState(true);
  const [storyEnded, setStoryEnded] = useState(false);
  const [playerSession, setPlayerSession] = useState<PlayerSessionFromApi | null>(null);
  const [character, setCharacter] = useState<Character | null>(null);
  const [poses, setPoses] = useState<CharacterPose[]>([]);
  const [errorMsg, setErrorMsg] = useState<string | null>(null);
  const [isEditingCharacter, setIsEditingCharacter] = useState(false);

  const navigate = useNavigate();

  // ---------------- RESTART ----------------
  const handleRestart = () => navigate("/");

  // ---------------- LOAD SESSION & CHARACTER ----------------
  useEffect(() => {
    const loadSession = async () => {
      try {
        const sessionData = await getSession(sessionId);
        setPlayerSession(sessionData);

        if (sessionData?.sessionId) {
          const charData = await getCharacterForSession(sessionData.sessionId);
          setCharacter(charData);
        }
      } catch (err) {
        console.error("Failed to load session or character", err);
        setErrorMsg("Failed to load session or character data.");
      }
    };
    loadSession();
  }, [sessionId]);

  // ---------------- LOAD CURRENT ACT ----------------
  const loadAct = async () => {
    setLoading(true);
    setErrorMsg(null);
    try {
      const result = await getCurrentAct(sessionId);
      if (!result?.act) {
        setErrorMsg(`No act found for session ${sessionId}.`);
        setCurrentAct(null);
        setChoices([]);
        return;
      }

      setCurrentAct(result.act);
      setChoices(result.act.choices || []);
      setStoryEnded(result.session?.isCompleted ?? false);
    } catch (err) {
      console.error("Error loading act:", err);
      setErrorMsg("Failed to load act.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadAct();
  }, [sessionId]);

  // ---------------- FETCH POSES ----------------
  useEffect(() => {
    const fetchPoses = async () => {
      try {
        const fetchedPoses = await getAllPoses();
        setPoses(fetchedPoses);
      } catch (err) {
        console.error("Failed to fetch poses:", err);
      }
    };
    fetchPoses();
  }, []);

  // ---------------- HANDLE CHOICE ----------------
  const handleChoiceClick = async (nextActNumber: number) => {
  setLoading(true);
  try {
    await moveToNextAct(sessionId, nextActNumber);
    await loadAct();
  } catch (err) {
    console.error("Error advancing act:", err);
    setErrorMsg("Failed to advance to next act.");
  } finally {
    setLoading(false);
  }
};

  if (loading) return <div>Loading...</div>;
  if (errorMsg) return <div className="error">{errorMsg}</div>;
  if (!currentAct) return <div>No act data available.</div>;

  const selectedPose = poses.find((p) => p.id === character?.poseId);

  return (
    <div style={{ minHeight: "100vh", backgroundColor: storyEnded ? "#7c372fff" : "#f0f8ff" }}>
      {/* Navbar */}
      <nav
        style={{
          width: "100%",
          padding: "10px 20px",
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          backgroundColor: "#333",
          color: "white",
          position: "sticky",
          top: 0,
          zIndex: 1000,
        }}
      >
        <span style={{ fontWeight: "bold" }}>The Fallen Dragon</span>
        {character && (
          <button
            onClick={() => setIsEditingCharacter(true)}
            style={{
              padding: "6px 12px",
              backgroundColor: "#4caf50",
              color: "white",
              border: "none",
              borderRadius: "5px",
              cursor: "pointer",
            }}
          >
            Edit Character
          </button>
        )}
      </nav>

      {storyEnded ? (
        <EndingScreen onRestart={handleRestart} />
      ) : (
        <>
          {/* Story content */}
          <h2>Act {currentAct.actNumber}</h2>
          <p>{currentAct.text}</p>

          {/* Choices */}
          <div className="choices">
            {choices.length > 0 ? (
              choices.map((choice) => (
                <button
                  key={choice.choiceId}
                  onClick={() => handleChoiceClick(choice.nextActNumber)}
                  style={{
                    margin: "5px",
                    padding: "10px 20px",
                    backgroundColor: "#007bff",
                    color: "white",
                    border: "none",
                    borderRadius: "5px",
                    cursor: "pointer",
                  }}
                >
                  {choice.text}
                </button>
              ))
            ) : (
              <p>No choices available.</p>
            )}
          </div>

          {/* Character preview */}
          {playerSession && character && (
            <div style={{ marginTop: "20px" }}>
              <p>{playerSession.characterName}</p>
              <div style={{ width: "100px", height: "100px", position: "relative", margin: "10px 0" }}>
                <img
                  src="/images/base.png"
                  alt="base"
                  style={{ position: "absolute", width: "100%", height: "100%", objectFit: "contain" }}
                />
                {character.hair && (
                  <img
                    src={`/images/hair/${character.hair}`}
                    alt="hair"
                    style={{ position: "absolute", width: "100%", height: "100%", objectFit: "contain" }}
                  />
                )}
                {character.face && (
                  <img
                    src={`/images/faces/${character.face}`}
                    alt="face"
                    style={{ position: "absolute", width: "100%", height: "100%", objectFit: "contain" }}
                  />
                )}
                {character.outfit && (
                  <img
                    src={`/images/clothes/${character.outfit}`}
                    alt="clothing"
                    style={{ position: "absolute", width: "100%", height: "100%", objectFit: "contain" }}
                  />
                )}
                {selectedPose && (
                  <img
                    src={`/images/poses/${selectedPose.imageUrl}`}
                    alt="pose"
                    style={{ position: "absolute", width: "100%", height: "100%", objectFit: "contain" }}
                  />
                )}
              </div>
            </div>
          )}

          {/* Character Edit Modal */}
          {isEditingCharacter && character && (
            <div
              style={{
                position: "fixed",
                top: 0,
                left: 0,
                width: "100%",
                height: "100%",
                backgroundColor: "rgba(0,0,0,0.6)",
                display: "flex",
                justifyContent: "center",
                alignItems: "center",
                zIndex: 2000,
              }}
              onClick={() => setIsEditingCharacter(false)}
            >
              <div
                onClick={(e) => e.stopPropagation()}
                style={{ backgroundColor: "white", padding: "20px", borderRadius: "8px", maxWidth: "500px", width: "90%" }}
              >
                <CharacterBuilder
                  character={character}
                  poses={poses}
                  onHairChange={(hair) =>
                    setCharacter((prev) => (prev ? { ...prev, hair } : prev))
                  }
                  onFaceChange={(face) =>
                    setCharacter((prev) => (prev ? { ...prev, face } : prev))
                  }
                  onOutfitChange={(outfit) =>
                    setCharacter((prev) => (prev ? { ...prev, outfit } : prev))
                  }
                  onPoseChange={(poseId) =>
                    setCharacter((prev) => (prev ? { ...prev, poseId: poseId ?? null } : prev))
                  }
                />

                <button
                  onClick={async () => {
                    setIsEditingCharacter(false);
                    try {
                      if (character) {
                        await updateCharacter(sessionId, character);
                        console.log("Character saved.");
                      }
                    } catch (err) {
                      console.error("Failed to save character:", err);
                    }
                  }}
                  style={{ marginTop: "10px", padding: "8px 12px", backgroundColor: "#333", color: "white", border: "none", borderRadius: "5px", cursor: "pointer" }}
                >
                  Close & Save
                </button>
              </div>
            </div>
          )}
        </>
      )}
    </div>
  );
};

export default StoryPage;
