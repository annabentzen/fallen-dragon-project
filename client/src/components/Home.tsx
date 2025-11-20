import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { createSession } from "../services/storyApi";
import {
  getAllPoses
} from "../services/characterApi";
import CharacterBuilder from "./CharacterBuilder";
import { Character, CharacterPose } from "../types/character";
import styles from "../styles/Home.module.css";

export default function Home() {
  const navigate = useNavigate();

  // Character state
  const [characterName, setCharacterName] = useState("");
  const [character, setCharacter] = useState<Character>({
    head: "mage-head1.png",
    body: "knight-body.png",
    poseId: null,
    id: 0, // placeholder, backend will assign real id
  });

  // UI state
  const [poses, setPoses] = useState<CharacterPose[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Load poses on mount
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

  // Reset character to defaults
  const resetCharacter = () => {
    setCharacterName("");
    setCharacter({
      head: "mage-head1.png",
      body: "knight-body.png",
      poseId: null,
      id: 0,
    });
    setError(null);
  };

  // Start the story
const startStory = async () => {
  if (!characterName.trim()) {
    setError("Please enter a hero name");
    return;
  }
  if (character.poseId === null) {
    setError("Please select a pose");
    return;
  }

  setLoading(true);
  setError(null);

  try {
    // Only send the name — backend creates character from builder state
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
    <div className={styles.container}>
      <h1 className={styles.title}>The Fallen Dragon</h1>

      <div className={styles.introSection}>
        <p className={styles.introText}>
          A dragon has fallen and landed in your village. Hero, will you use your special powers to save
          it? The village is depending on you. Your choices will determine its fate.
        </p>
        <p className={styles.introText}>
          <span className={styles.introHighlight}>
            Create your hero and begin your journey...
          </span>
        </p>
      </div>

      {error && <div className={styles.errorMessage}>{error}</div>}

      <div className={styles.heroNameSection}>
        <label className={styles.label}>Hero name:</label>
        <input
          type="text"
          placeholder="Enter your hero-name"
          value={characterName}
          onChange={(e) => setCharacterName(e.target.value)}
          className={styles.input}
          disabled={loading}
        />
      </div>

      <CharacterBuilder
        character={character}
        poses={poses}
        onHeadChange={(head) => setCharacter((prev) => ({ ...prev, head }))}
        onBodyChange={(body) => setCharacter((prev) => ({ ...prev, body }))}
        onPoseChange={(poseId) => setCharacter((prev) => ({ ...prev, poseId }))}
      />

      <button
        onClick={startStory}
        disabled={loading}
        className={styles.buttonPrimary}
      >
        {loading ? "Starting..." : "Start Mission"}
      </button>

      <button
        onClick={resetCharacter}
        disabled={loading}
        className={styles.buttonSecondary}
      >
        Reset Character
      </button>
    </div>
  );
}