import React, { useEffect, useState } from "react";
import EndingScreen from "./EndingScreen";
import { getCurrentAct, getSession, updateCharacterDesign } from "../services/storyApi";
import { getAllPoses } from "../services/characterApi";
import {
  Act,
  Choice,
  PlayerSessionFromApi,
  CharacterDesign,
  CharacterPose,
} from "../types/story";
import CharacterBuilder from "./CharacterBuilder";
import { useNavigate } from "react-router-dom";
import axios from "axios";

interface StoryPageProps {
  sessionId: number;
}

// ---------- HELPER FUNCTION ----------
function safeParseCharacterDesign(
  data: CharacterDesign | null | undefined
): CharacterDesign {
  if (!data) return {};
  return {
    hair: data.hair ?? undefined,
    face: data.face ?? undefined,
    outfit: data.outfit ?? undefined,
    poseId: data.poseId ?? undefined,
  };
}

const StoryPage: React.FC<StoryPageProps> = ({ sessionId }) => {
  const [currentAct, setCurrentAct] = useState<Act | null>(null);
  const [choices, setChoices] = useState<Choice[]>([]);
  const [loading, setLoading] = useState(true);
  const [storyEnded, setStoryEnded] = useState(false);
  const [playerSession, setPlayerSession] = useState<PlayerSessionFromApi | null>(null);
  const [characterDesign, setCharacterDesign] = useState<CharacterDesign>({});
  const [poses, setPoses] = useState<CharacterPose[]>([]);
  const [errorMsg, setErrorMsg] = useState<string | null>(null);
  const [isEditingCharacter, setIsEditingCharacter] = useState(false);

  const navigate = useNavigate();

  // ---------- RESTART ----------
  const handleRestart = () => {
    navigate("/");
  };

  // ---------- LOAD SESSION ----------
  useEffect(() => {
    const loadSession = async () => {
      try {
        const sessionData = await getSession(sessionId);
        setPlayerSession(sessionData);
        setCharacterDesign({
          hair: sessionData.hair,
          face: sessionData.face,
          outfit: sessionData.outfit,
          poseId: sessionData.poseId,
        });
      } catch (err) {
        console.error("Failed to load session", err);
        setErrorMsg("Failed to load session data.");
      }
    };
    loadSession();
  }, [sessionId]);

  // ---------- LOAD CURRENT ACT ----------
  const loadAct = async () => {
    setLoading(true);
    setErrorMsg(null);
    try {
      const result = await getCurrentAct(sessionId);
      if (!result || !result.act) {
        setErrorMsg(`No act found for session ${sessionId}.`);
        setCurrentAct(null);
        setChoices([]);
        return;
      }

      const { session, act } = result;
      setCurrentAct(act);
      setChoices(act.choices || []);
      setStoryEnded(session?.isCompleted ?? false);

      if (session) {
        const parsedDesign = safeParseCharacterDesign({
          hair: session.hair,
          face: session.face,
          outfit: session.outfit,
          poseId: session.poseId,
        });
        setPlayerSession(session);
        setCharacterDesign(parsedDesign);
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

  // ---------- HANDLE CHOICE ----------
  const handleChoiceClick = async (nextActNumber: number) => {
    setLoading(true);
    try {
      await axios.post(
        `http://localhost:5151/api/story/nextAct/${sessionId}`,
        { nextActNumber },
        { headers: { "Content-Type": "application/json" } }
      );
      await loadAct();
    } catch (error) {
      console.error("Error advancing act:", error);
      setErrorMsg("Failed to advance to next act.");
    } finally {
      setLoading(false);
    }
  };

  // ---------- FETCH POSES ----------
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

  // ---------- RENDER ----------
  if (loading) return <div>Loading...</div>;
  if (errorMsg) return <div className="error">{errorMsg}</div>;
  if (!currentAct) return <div>No act data available.</div>;

  const selectedPose = poses.find((p) => Number(p.id) === Number(characterDesign.poseId));

  return (
    <div
      style={{
        minHeight: "100vh",
        backgroundColor: storyEnded ? "#7c372fff" : "#f0f8ff",
      }}
    >
      {/* --- NAVBAR --- */}
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
      </nav>

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

          {/* --- Character Corner --- */}
          {playerSession && (
            <div style={{ marginTop: "20px" }}>
              <p>{playerSession.characterName}</p>
              <div
                style={{
                  width: "100px",
                  height: "100px",
                  position: "relative",
                  margin: "10px 0",
                }}
              >
                <img
                  src="/images/base.png"
                  alt="base"
                  style={{ position: "absolute", width: "100%", height: "100%", objectFit: "contain" }}
                />
                {characterDesign.hair && (
                  <img
                    src={`/images/hair/${characterDesign.hair}`}
                    alt="hair"
                    style={{ position: "absolute", width: "100%", height: "100%", objectFit: "contain" }}
                  />
                )}
                {characterDesign.face && (
                  <img
                    src={`/images/faces/${characterDesign.face}`}
                    alt="face"
                    style={{ position: "absolute", width: "100%", height: "100%", objectFit: "contain" }}
                  />
                )}
                {characterDesign.outfit && (
                  <img
                    src={`/images/clothes/${characterDesign.outfit}`}
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

          {/* --- Character Edit Modal --- */}
          {isEditingCharacter && (
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
                style={{
                  backgroundColor: "white",
                  padding: "20px",
                  borderRadius: "8px",
                  maxWidth: "500px",
                  width: "90%",
                }}
              >
                <CharacterBuilder
                  hair={characterDesign.hair || "hair1.png"}
                  face={characterDesign.face || "face1.png"}
                  outfit={characterDesign.outfit || "clothing1.png"}
                  poseId={characterDesign.poseId ?? null}
                  poses={poses}
                  onHairChange={(hair) => setCharacterDesign((prev) => ({ ...prev, hair }))}
                  onFaceChange={(face) => setCharacterDesign((prev) => ({ ...prev, face }))}
                  onOutfitChange={(outfit) => setCharacterDesign((prev) => ({ ...prev, outfit }))}
                  onPoseChange={(poseId) =>
                    setCharacterDesign((prev) => ({ ...prev, poseId: poseId ?? undefined }))
                  }
                />
                <button
                  onClick={async () => {
                    setIsEditingCharacter(false);
                    try {
                      await updateCharacterDesign(sessionId, characterDesign);
                      console.log("Character design saved.");
                      // Update session state after save
                      setPlayerSession((prev) =>
                        prev
                          ? { ...prev, ...characterDesign }
                          : prev
                      );
                    } catch (err) {
                      console.error("Failed to save character:", err);
                    }
                  }}
                  style={{
                    marginTop: "10px",
                    padding: "8px 12px",
                    backgroundColor: "#333",
                    color: "white",
                    border: "none",
                    borderRadius: "5px",
                    cursor: "pointer",
                  }}
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
