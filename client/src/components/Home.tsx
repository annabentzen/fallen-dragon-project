import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { createSession } from "../services/storyApi";
import { getAllPoses } from "../services/characterApi";
import CharacterBuilder from "./CharacterBuilder";
import { Character, CharacterPose } from "../types/character";

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
      hair: "hair1.png",
      face: "face1.png",
      outfit: "clothing1.png",
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
      const session = await createSession({
        characterName,
        character,
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
    <div style={{ maxWidth: "800px", margin: "0 auto", padding: "20px" }}>
      <h1 style={{ textAlign: "center", color: "#333" }}>The Fallen Dragon</h1>

      <div
        style={{
          textAlign: "center",
          margin: "20px 0",
          padding: "15px",
          backgroundColor: "#f5f5f5",
          borderRadius: "8px",
        }}
      >
        <p>
          A dragon has fallen and landed in your village. Hero, will you save it? Your choices will
          determine its fate.
        </p>
        <p>
          <strong>Create your hero and begin your journey...</strong>
        </p>
      </div>

      {error && (
        <div
          style={{
            backgroundColor: "#ffebee",
            color: "#c62828",
            padding: "10px",
            borderRadius: "4px",
            marginBottom: "15px",
          }}
        >
          {error}
        </div>
      )}

      <div style={{ marginBottom: "20px" }}>
        <label style={{ display: "block", marginBottom: "5px", fontWeight: "bold" }}>
          Hero Name:
        </label>
        <input
          type="text"
          placeholder="Enter your hero's name"
          value={characterName}
          onChange={(e) => setCharacterName(e.target.value)}
          style={{
            width: "100%",
            padding: "10px",
            fontSize: "16px",
            border: "1px solid #ccc",
            borderRadius: "4px",
          }}
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
        style={{
          width: "100%",
          padding: "15px",
          fontSize: "18px",
          fontWeight: "bold",
          backgroundColor: loading ? "#ccc" : "#4CAF50",
          color: "white",
          border: "none",
          borderRadius: "8px",
          cursor: loading ? "not-allowed" : "pointer",
          marginTop: "20px",
        }}
      >
        {loading ? "Starting..." : "Start Mission"}
      </button>

      <button
        onClick={resetCharacter}
        disabled={loading}
        style={{
          width: "100%",
          padding: "12px",
          fontSize: "16px",
          backgroundColor: "transparent",
          color: "#666",
          border: "2px solid #ccc",
          borderRadius: "8px",
          cursor: loading ? "not-allowed" : "pointer",
          marginTop: "10px",
        }}
      >
        Reset Character
      </button>
    </div>
  );
}

