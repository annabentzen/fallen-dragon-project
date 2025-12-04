import React, { useEffect, useState } from "react";
import EndingScreen from "./EndingScreen";
import CharacterBuilder from "./CharacterBuilder";
import { useNavigate } from "react-router-dom";
import styles from "../styles/Story.module.css";
import { removeToken } from "../services/authApi";

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
import Navbar from "./Navbar";

interface StoryPageProps {
  sessionId: number;
}

const StoryPage: React.FC<StoryPageProps> = ({ sessionId }) => {
  const [currentAct, setCurrentAct] = useState<Act | null>(null);
  const [choices, setChoices] = useState<Choice[]>([]);
  const [loading, setLoading] = useState(true);
  const [playerSession, setPlayerSession] =
    useState<PlayerSessionFromApi | null>(null);
  const [character, setCharacter] = useState<Character | null>(null);
  const [poses, setPoses] = useState<CharacterPose[]>([]);
  const [errorMsg, setErrorMsg] = useState<string | null>(null);
  const [username, setUsername] = useState<string>("");
  const [showCharacterBuilder, setShowCharacterBuilder] = useState(false);

  const navigate = useNavigate();

  // Act numbers map to correct story ending based on narrative branches
  const getEndingType = (
    actNumber: number
  ):
    | "heroDeath"
    | "dragonKilled"
    | "tragedy"
    | "ignored"
    | "recovery"
    | "guardian"
    | "default" => {
    if ([113, 121, 1112].includes(actNumber)) return "heroDeath";
    if ([1311, 1111221].includes(actNumber)) return "dragonKilled";
    if ([1321, 1111232].includes(actNumber)) return "tragedy";
    if (actNumber === 122) return "ignored";
    if (actNumber === 1111211) return "recovery";
    if (actNumber === 1111231) return "guardian";
    return "default";
  };

  const handleRestart = () => {
    navigate("/", { replace: true });
  };

  useEffect(() => {
    if (playerSession) {
      setUsername(playerSession.characterName);
    }
  }, [playerSession]);

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

      const mappedAct: Act = {
        actNumber: actDto.actNumber,
        text: actDto.text,
        choices: actDto.choices.map((c, index) => ({
          choiceId: index,
          text: c.text,
          nextActNumber: c.nextActNumber,
        })),
        isEnding: actDto.isEnding,
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

  const handleLogout = () => {
    removeToken();
    navigate("/");
  };

  if (loading) return <div className={styles.loading}>Loading...</div>;
  if (errorMsg) return <div className={styles.error}>{errorMsg}</div>;
  if (!currentAct)
    return <div className={styles.noData}>No act data available.</div>;

  const selectedPose = poses.find((p) => p.id === character?.poseId);
  const isEnding = choices.length === 0;

  return (
    <div className={styles.storyContainer}>
      <Navbar
        onOpenCharacterBuilder={() => setShowCharacterBuilder(true)}
        showCharacterButton={true}
      />

      {isEnding && currentAct && (
        <EndingScreen
          endingType={getEndingType(currentAct.actNumber)}
          endingText={currentAct.text}
          onRestart={handleRestart}
          sessionId={sessionId}
        />
      )}

      {!isEnding && currentAct && (
        <>
          <div className={styles.atmosphereOverlay} />
          <div className={styles.storyScene}>
            <div className={styles.noteboardSection}>
              <div className={styles.noteboardContainer}>
                <div className={styles.actText}>{currentAct.text}</div>
              </div>

              <div className={styles.choicesContainer}>
                {choices.map((choice, index) => (
                  <div
                    key={choice.choiceId}
                    onClick={() => handleChoiceClick(choice.nextActNumber)}
                    className={styles.choiceButton}
                    style={{ animationDelay: `${index * 0.1}s` }}
                  >
                    <span className={styles.choiceText}>{choice.text}</span>
                  </div>
                ))}
              </div>
            </div>

            <div className={styles.dragonSection}>
              <img
                src="/images/game-images/ui/Death3.png"
                alt="Dragon"
                className={styles.dragonImage}
              />
            </div>

            {playerSession && character && (
              <div className={styles.characterSection}>
                <div className={styles.characterNameBadge}>
                  <p className={styles.characterName}>
                    {playerSession.characterName}
                  </p>
                </div>

                <div className={styles.characterPreview}>
                  {/* Body only show if no pose is selected */}
                  {!character.poseId && character.body && (
                    <img
                      src={`/images/avatar/body/${character.body}`}
                      alt="body"
                      className={`${styles.characterLayer} ${
                        styles[
                          `body${
                            character.body
                              .split("-")[0]
                              .charAt(0)
                              .toUpperCase() +
                            character.body.split("-")[0].slice(1)
                          }`
                        ]
                      }`}
                    />
                  )}

                  {character.head && (
                    <img
                      src={`/images/avatar/heads/${character.head}`}
                      alt="head"
                      className={styles.characterLayer}
                    />
                  )}

                  {character.poseId && selectedPose && (
                    <img
                      src={`/images/avatar/poses/${selectedPose.imageUrl}`}
                      alt="pose"
                      className={`${styles.characterLayer} ${
                        styles[`pose${selectedPose.name.replace(/\s+/g, "")}`]
                      }`}
                    />
                  )}
                </div>
              </div>
            )}
          </div>

          {showCharacterBuilder && character && (
            <div
              className={styles.modalOverlay}
              onClick={() => setShowCharacterBuilder(false)}
            >
              <div
                className={styles.modalContent}
                onClick={(e) => e.stopPropagation()}
              >
                <h2 className={styles.modalTitle}>Edit Your Character</h2>
                <CharacterBuilder
                  character={character}
                  poses={poses}
                  onHeadChange={(head) =>
                    setCharacter((prev) => (prev ? { ...prev, head } : prev))
                  }
                  onBodyChange={(body) =>
                    setCharacter((prev) => (prev ? { ...prev, body } : prev))
                  }
                  onPoseChange={(poseId) =>
                    setCharacter((prev) =>
                      prev ? { ...prev, poseId: poseId ?? null } : prev
                    )
                  }
                />

                <div className={styles.modalButtonContainer}>
                  <button
                    onClick={async () => {
                      setShowCharacterBuilder(false);
                      if (character) {
                        await updateCharacter(sessionId, character);
                      }
                    }}
                    className={styles.closeButton}
                  >
                    Save & Close
                  </button>
                </div>
              </div>
            </div>
          )}
        </>
      )}
    </div>
  );
};
export default StoryPage;
