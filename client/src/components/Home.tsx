import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { createSession, safeParseCharacterDesign } from "../services/storyApi";
import {
  getAllPoses,
  saveCharacterToLocal,
  clearSavedCharacter,
  loadCharacterFromLocal,
} from "../services/characterApi";
import CharacterBuilder from "./CharacterBuilder";
import { Character, CharacterPose } from "../types/character";
import styles from "../styles/Home.module.css";

export default function Home() {
  const navigate = useNavigate();

  // Character state
  const [characterName, setCharacterName] = useState("");
  const [character, setCharacter] = useState<Character>({
    hair: "hair1.png",
    face: "face1.png",
    outfit: "clothing1.png",
    poseId: null,
    id: 0, // placeholder, backend will assign real id
  });

  // UI state
  const [poses, setPoses] = useState<CharacterPose[]>([]);
  const selectedPose = poses.find(pose => pose.id === character.poseId);
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

    const loadLastCharacter = () => {
      const lastCharacter = loadCharacterFromLocal();
      console.log("Loaded last character from localStorage:", lastCharacter);

      if (lastCharacter) {
        const parsedDesign = safeParseCharacterDesign(lastCharacter);
        if (parsedDesign.hair) setHair(parsedDesign.hair);
        if (parsedDesign.face) setFace(parsedDesign.face);
        if (parsedDesign.outfit) setOutfit(parsedDesign.outfit);
        if (parsedDesign.poseId !== undefined) setPoseId(parsedDesign.poseId);
      }
    };

    fetchPoses();
  }, []);

  // Reset character to defaults
  const resetCharacter = () => {
    setCharacterName("");
    setCharacter({
      hair: "hair1.png",
      face: "face1.png",
      outfit: "clothing1.png",
      poseId: null,
      id: 0,
    });
    console.log("Resetting character...");
    clearSavedCharacter();
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
    const session = await createSession(characterName.trim());
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
        onHairChange={(hair) => setCharacter((prev) => ({ ...prev, hair }))}
        onFaceChange={(face) => setCharacter((prev) => ({ ...prev, face }))}
        onOutfitChange={(outfit) => setCharacter((prev) => ({ ...prev, outfit }))}
        onPoseChange={(poseId) => setCharacter((prev) => ({ ...prev, poseId }))}
      />

      <button
        onClick={startStory}
        disabled={loading}
        className={styles.buttonPrimary}
      >
        {loading ? "Starting..." : "Start Mission"}
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

