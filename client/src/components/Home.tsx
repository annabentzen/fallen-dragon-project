import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { createSession } from "../services/storyApi";
import { getAllPoses } from "../services/characterApi";
import CharacterBuilder from "./CharacterBuilder";
import { Character, CharacterPose } from "../types/character";
import styles from "../styles/Home.module.css";

export default function Home() {
  const navigate = useNavigate();

  const [characterName, setCharacterName] = useState("");
  const [character, setCharacter] = useState<Character>({
    head: "mage1-head.png",
    body: "knight-body.png",
    poseId: null,
    id: 0,
  });

  const [poses, setPoses] = useState<CharacterPose[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchPoses = async () => {
      try {
        const fetchedPoses = await getAllPoses();
        console.log("Fetched poses for Home:", fetchedPoses);
        setPoses(fetchedPoses);
      } catch (err) {
        console.error("Failed to fetch poses for Home:", err);
      }
    };
    fetchPoses();
  }, []);

  const resetCharacter = () => {
    setCharacterName("");

    const standingPose = poses.find(
      (pose) => pose.name.toLowerCase() === "standing"
    );

    setCharacter({
      head: "mage1-head.png",
      body: "knight-body.png",
      poseId: null,
      id: 0,
    });
    setError(null);
  };

  // Start the story
  const startStory = async () => {
    if (!characterName.trim()) {
      setError("Enter hero name");
      return;
    }

    setLoading(true);
    setError(null);

    try {
      const session = await createSession(characterName.trim(), character);
      console.log("Session created:", session);
      navigate(`/story/${session.sessionId}`);
    } catch (err) {
      console.error("Failed to start story:", err);
      setError("Failed to start story. Is backend running?");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className={styles.pageWrapper}>
      <div className={styles.container}>
        <h1 className={styles.title}>The Fallen Dragon</h1>

        <div className={styles.mainContent}>
          {/* LEFT COLUMN - CHARACTER CREATOR */}
          <div className={styles.leftColumn}>
            <h2 className={styles.header}>Change your look</h2>
            {/* Hero Name Input */}
            <div className={styles.heroNameSection}>
              <label className={styles.label}></label>
              <input
                type="text"
                placeholder="Enter hero-name"
                value={characterName}
                onChange={(e) => setCharacterName(e.target.value)}
                className={styles.input}
                disabled={loading}
              />
              {error && <div className={styles.errorMessage}>{error}</div>}
            </div>
            <CharacterBuilder
              character={character}
              poses={poses}
              onHeadChange={(head) =>
                setCharacter((prev) => ({ ...prev, head }))
              }
              onBodyChange={(body) =>
                setCharacter((prev) => ({
                  ...prev,
                  body,
                  poseId: null, // Reset pose when body changes
                }))
              }
              onPoseChange={(poseId) =>
                setCharacter((prev) => ({ ...prev, poseId }))
              }
            />
          </div>

          {/* RIGHT COLUMN - MISSION BRIEFING */}
          <div className={styles.rightColumn}>
            <div className={styles.missionBriefing}>
              <h2 className={styles.header}>Mission</h2>
              <p className={styles.introText}>
                A dragon has fallen and landed in your village. Hero, will you
                use your special powers to save it? The village is depending on
                you. Your choices will determine its fate.
              </p>
              <p className={styles.introText}>
                <span className={styles.introHighlight}>
                  Create your hero and begin your journey...
                </span>
              </p>

              {/* Begin Mission - button */}
              <div className={styles.actionSection}>
                <button
                  onClick={startStory}
                  disabled={loading}
                  className={styles.beginButton}
                >
                  {loading ? "Starting..." : "Begin Mission"}
                </button>
              </div>
            </div>
          </div>
        </div>

        {/* Reset Character Button - fixed to bottom left */}
        <button
          onClick={resetCharacter}
          disabled={loading}
          className={styles.resetButton}
        >
          Reset Character
        </button>
      </div>
    </div>
  );
}
