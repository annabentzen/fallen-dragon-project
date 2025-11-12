import styles from "../styles/Home.module.css";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { createSession, safeParseCharacterDesign } from "../services/storyApi";
import {
  getAllPoses,
  CharacterPose,
  saveCharacterToLocal,
  clearSavedCharacter,
  loadCharacterFromLocal,
} from "../services/characterApi";
import CharacterBuilder from "../components/CharacterBuilder";

export default function Home() {
  const navigate = useNavigate();

  // Character state
  const [characterName, setCharacterName] = useState("");
  const [hair, setHair] = useState("hair1.png");
  const [face, setFace] = useState("face1.png");
  const [outfit, setOutfit] = useState("clothing1.png");
  const [poseId, setPoseId] = useState<number | null>(null);

  // UI state
  const [poses, setPoses] = useState<CharacterPose[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Load poses and last character on mount
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
    loadLastCharacter();
  }, []);

  // Reset character to defaults
  const resetCharacter = () => {
    console.log("Resetting character...");
    clearSavedCharacter();
    setCharacterName("");
    setHair("hair1.png");
    setFace("face1.png");
    setOutfit("clothing1.png");
    setPoseId(null);
    setError(null);
  };

  // Start the story
  const startStory = async () => {
    if (!characterName.trim()) {
      setError("Please enter a hero name");
      return;
    }
    if (!poseId) {
      setError("Please select a pose");
      return;
    }

    setLoading(true);
    setError(null);

    try {
      const character = { hair, face, outfit, poseId };
      saveCharacterToLocal(character);
      console.log("Character saved to localStorage:", character);

      const session = await createSession({
        characterName,
        characterDesign: character,
        storyId: 1,
      });
      console.log("Session created:", session);

      navigate(`/story/${session.sessionId}`);
    } catch (err) {
      console.error("Failed to start story:", err);
      setError("Failed to start story. Check if your backend is running.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className={styles.container}>
      <h1 className={styles.title}>The Fallen Dragon</h1>

      <div className={styles.introSection}>
        <p className={styles.introText}>
          A dragon has fallen and landed in your village. Hero, will you save
          it? Your choices will determine its fate.
        </p>
        <p className={styles.introText}>
          <span className={styles.introHighlight}>
            Create your hero and begin your journey...
          </span>
        </p>
      </div>

      {error && <div className={styles.errorMessage}>{error}</div>}

      <div className={styles.heroNameSection}>
        <label className={styles.label}>Hero Name:</label>
        <input
          type="text"
          placeholder="Enter your hero's name"
          value={characterName}
          onChange={(e) => setCharacterName(e.target.value)}
          className={styles.input}
          disabled={loading}
        />
      </div>

      <CharacterBuilder
        hair={hair}
        face={face}
        outfit={outfit}
        poseId={poseId}
        poses={poses}
        onHairChange={setHair}
        onFaceChange={setFace}
        onOutfitChange={setOutfit}
        onPoseChange={setPoseId}
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
