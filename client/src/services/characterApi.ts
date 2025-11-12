import axios from "axios";
import { CharacterDesign, PlayerSessionFromApi } from "../types/story";

const API_BASE = "http://localhost:5000/api"; // backend base
const POSE_API = "http://localhost:5151/api/poses"; // pose service (if separate)

// ---------------------------------------------------------------------------
// ✅ Safe JSON parser for character design
// ---------------------------------------------------------------------------
function safeParseCharacterDesign(data: string | CharacterDesign | undefined): CharacterDesign {
  if (!data) return {};
  if (typeof data === "string") {
    try {
      return JSON.parse(data);
    } catch {
      console.warn("Failed to parse characterDesign JSON:", data);
      return {};
    }
  }
  return data;
}

// ---------------------------------------------------------------------------
// ✅ LocalStorage helpers
// ---------------------------------------------------------------------------
export function saveCharacterToLocal(character: CharacterDesign) {
  try {
    localStorage.setItem("characterDesign", JSON.stringify(character));
  } catch (error) {
    console.error("Failed to save character:", error);
  }
}

export function loadCharacterFromLocal(): CharacterDesign | null {
  try {
    const saved = localStorage.getItem("characterDesign");
    if (!saved) return null;
    return safeParseCharacterDesign(saved);
  } catch (error) {
    console.error("Failed to load character:", error);
    return null;
  }
}

export function clearSavedCharacter(): void {
  localStorage.removeItem("characterDesign");
}

// ---------------------------------------------------------------------------
// ✅ Backend session management
// ---------------------------------------------------------------------------
export async function createSession(data: {
  characterName: string;
  characterDesign: CharacterDesign;
  storyId: number;
}): Promise<PlayerSessionFromApi> {
  try {
    const payload = {
      ...data,
      characterDesign: JSON.stringify(data.characterDesign),
    };

    const response = await axios.post(`${API_BASE}/story/create-session`, payload);
    return {
      ...response.data,
      characterDesign: safeParseCharacterDesign(response.data.characterDesign),
    };
  } catch (error) {
    console.error("Error creating session:", error);
    throw error;
  }
}

export async function getSession(id: number): Promise<PlayerSessionFromApi | null> {
  try {
    const response = await axios.get(`${API_BASE}/story/session/${id}`);
    const data = response.data;
    return {
      ...data,
      characterDesign: safeParseCharacterDesign(data.characterDesign),
    };
  } catch (error) {
    console.error("Error fetching session:", error);
    return null;
  }
}

// ---------------------------------------------------------------------------
// ✅ Pose API – fetch available poses (images)
// ---------------------------------------------------------------------------
export interface CharacterPose {
  id: number;
  name: string;
  imageUrl: string;
}

export async function getAllPoses(): Promise<CharacterPose[]> {
  try {
    const response = await axios.get(POSE_API);
    console.log("API response in getAllPoses():", response.data);

    // Handle various backend formats
    if (response.data && Array.isArray(response.data.poses)) {
      return response.data.poses;
    } else if (Array.isArray(response.data)) {
      return response.data;
    } else {
      console.warn("getAllPoses(): Unexpected response, returning empty array");
      return [];
    }
  } catch (err) {
    console.error("getAllPoses() failed:", err);
    return [];
  }
}
