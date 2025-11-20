import React, { useEffect, useState } from "react";
import EndingScreen from "./EndingScreen";
import CharacterBuilder from "./CharacterBuilder";
import { useNavigate } from "react-router-dom";

import {
  ActDto,
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
  const [playerSession, setPlayerSession] = useState<PlayerSessionFromApi | null>(null);
  const [character, setCharacter] = useState<Character | null>(null);
  const [poses, setPoses] = useState<CharacterPose[]>([]);
  const [errorMsg, setErrorMsg] = useState<string | null>(null);
  const [isEditingCharacter, setIsEditingCharacter] = useState(false);

  const navigate = useNavigate();

  // ---------------- DETERMINE ENDING TYPE ----------------
  const getEndingType = (actNumber: number): 
    'heroDeath' | 'dragonKilled' | 'tragedy' | 'ignored' | 'recovery' | 'guardian' | 'default' => 
  {
    if ([113, 121, 1112].includes(actNumber)) return "heroDeath";
    if ([1311, 1111221].includes(actNumber)) return "dragonKilled";
    if ([1321, 1111232].includes(actNumber)) return "tragedy";
    if (actNumber === 122) return "ignored";
    if (actNumber === 1111211) return "recovery";
    if (actNumber === 1111231) return "guardian";
    return "default";
  };


  // ---------------- RESTART ----------------
  const handleRestart = () => {
  // Simply navigate back to home page
  // This will force user to create a new session
  navigate("/", { replace: true });
};

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
      const actDto: ActDto = await getCurrentAct(sessionId);

      if (!actDto?.text) {
        setErrorMsg(`No act found for session ${sessionId}.`);
        setCurrentAct(null);
        setChoices([]);
        return;
      }

      // Map C# PascalCase → your frontend camelCase
      const mappedAct: Act = {
        actNumber: actDto.actNumber,
        text: actDto.text,
        choices: actDto.choices.map((c, index) => ({
          choiceId: index,
          text: c.text,
          nextActNumber: c.nextActNumber
        })),
        isEnding: actDto.isEnding
      };

      setCurrentAct(mappedAct);
      setChoices(mappedAct.choices);

    } catch (err) {
      console.error("Error loading act:", err);
      setErrorMsg("Failed to load the next part of the story...");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (sessionId) loadAct();
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

    // Determine if we're on an ending (no choices = ending)
  const isEnding = choices.length === 0;

  return (
    <div style={{ minHeight: "100vh", backgroundColor: "#f0f8ff" }}>
      {/* ==================== NAVBAR ==================== */}
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

        {/* Hide Edit button on ending screen */}
        {character && !isEnding && (
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

      {/* ==================== ENDING SCREEN ==================== */}
      {isEnding && currentAct && (
        <EndingScreen
          endingType={getEndingType(currentAct.actNumber)}
          endingText={currentAct.text}
          onRestart={handleRestart}
          navigate={navigate} 
        />
      )}

      {/* ==================== NORMAL STORY CONTENT ==================== */}
      {!isEnding && currentAct && (
        <>
          <div style={{ padding: "20px", maxWidth: "900px", margin: "0 auto" }}>
            <h2 style={{ textAlign: "center", marginBottom: "20px" }}>
              Act {currentAct.actNumber}
            </h2>
            <p style={{ fontSize: "18px", lineHeight: "1.7", marginBottom: "30px" }}>
              {currentAct.text}
            </p>

            {/* Choices */}
            <div
              style={{
                display: "flex",
                flexDirection: "column",
                gap: "12px",
                alignItems: "center",
                margin: "40px 0",
              }}
            >
              {choices.map((choice) => (
                <button
                  key={choice.choiceId}
                  onClick={() => handleChoiceClick(choice.nextActNumber)}
                  style={{
                    width: "100%",
                    maxWidth: "500px",
                    padding: "14px 20px",
                    backgroundColor: "#007bff",
                    color: "white",
                    border: "none",
                    borderRadius: "8px",
                    cursor: "pointer",
                    fontSize: "17px",
                    fontWeight: "600",
                    transition: "all 0.2s",
                  }}
                  onMouseOver={(e) => (e.currentTarget.style.backgroundColor = "#0056b3")}
                  onMouseOut={(e) => (e.currentTarget.style.backgroundColor = "#007bff")}
                >
                  {choice.text}
                </button>
              ))}
            </div>

            {/* Character Preview – FIXED */}
            {playerSession && character && !isEnding && (
              <div style={{ textAlign: "center", margin: "50px 0" }}>
                <p style={{ fontWeight: "bold", fontSize: "22px", marginBottom: "16px" }}>
                  {playerSession.characterName}
                </p>
                <div
                  style={{
                    width: "280px",
                    height: "280px",
                    position: "relative",
                    margin: "0 auto",
                    backgroundColor: "#fff",
                    border: "4px solid #333",
                    borderRadius: "16px",
                    overflow: "hidden",
                    boxShadow: "0 8px 20px rgba(0,0,0,0.2)",
                  }}
                >
                  {/* Base body */}
                  <img
                    src="/images/base.png"
                    alt="base"
                    style={{ position: "absolute", inset: 0, width: "100%", height: "100%", objectFit: "contain" }}
                  />
                  {/* Hair */}
                  {character.hair && (
                    <img
                      src={`/images/hair/${character.hair}`}
                      alt="hair"
                      style={{ position: "absolute", inset: 0, width: "100%", height: "100%", objectFit: "contain" }}
                    />
                  )}
                  {/* Face */}
                  {character.face && (
                    <img
                      src={`/images/faces/${character.face}`}
                      alt="face"
                      style={{ position: "absolute", inset: 0, width: "100%", height: "100%", objectFit: "contain" }}
                    />
                  )}
                  {/* Outfit */}
                  {character.outfit && (
                    <img
                      src={`/images/clothes/${character.outfit}`}
                      alt="outfit"
                      style={{ position: "absolute", inset: 0, width: "100%", height: "100%", objectFit: "contain" }}
                    />
                  )}
                  {/* Pose (on top) */}
                  {character.poseId && selectedPose && (
                    <img
                      src={`/images/poses/${selectedPose.imageUrl}`}
                      alt="pose"
                      style={{ position: "absolute", inset: 0, width: "100%", height: "100%", objectFit: "contain" }}
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
                backgroundColor: "rgba(0,0,0,0.7)",
                display: "flex",
                justifyContent: "center",
                alignItems: "center",
                zIndex: 2000,
              }}
              onClick={() => setIsEditingCharacter(false)}
            >
              <div
                onClick={(e) => e.stopPropagation()}
                style={{
                  backgroundColor: "white",
                  padding: "30px",
                  borderRadius: "12px",
                  maxWidth: "560px",
                  width: "90%",
                  maxHeight: "90vh",
                  overflowY: "auto",
                }}
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

                <div style={{ textAlign: "center", marginTop: "20px" }}>
                  <button
                    onClick={async () => {
                      setIsEditingCharacter(false);
                      if (character) {
                        await updateCharacter(sessionId, character);
                      }
                    }}
                    style={{
                      padding: "12px 24px",
                      backgroundColor: "#333",
                      color: "white",
                      border: "none",
                      borderRadius: "8px",
                      cursor: "pointer",
                      fontSize: "16px",
                    }}
                  >
                    Close and Save
                    </button>
                  </div>
                </div>
              </div>
            )}
          </div>
        </>
      )}
    </div>
  );
};

export default StoryPage;
