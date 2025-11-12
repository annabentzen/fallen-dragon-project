// src/services/storyApi.ts
import axios from "axios";
import { Act, PlayerSessionFromApi } from "../types/story";

// ✅ REMOVED: circular import from StoryPage
// import parseCharacterDesign from "../components/StoryPage";

const API_BASE = "http://localhost:5151/api/story";

// -----------------------------------------
// Interfaces
// -----------------------------------------
export interface CharacterDto {
  hair?: string;
  face?: string;
  outfit?: string;
  poseId?: number;
}


// Parse a character design string or object safely
export function safeParseCharacterDesign(input: any) {
  if (!input) return {};
  if (typeof input === "string") {
    try {
      const parsed = JSON.parse(input);
      return parsed && typeof parsed === "object" ? parsed : {};
    } catch {
      console.warn("Failed to parse character design JSON:", input);
      return {};
    }
  }
  if (typeof input === "object") return input;
  return {};
}


// -----------------------------------------
// Create new story session
// -----------------------------------------
export const createSession = async (data: {
  characterName: string;
  character: CharacterDto;
  storyId: number;
}) => {
  const response = await axios.post(
    `${API_BASE}/start`,
    data,
    { headers: { "Content-Type": "application/json" } }
  );
  return response.data;
};



// -----------------------------------------
// Fetch existing session by ID
// -----------------------------------------
export const getSession = async (
  sessionId: number
): Promise<PlayerSessionFromApi> => {
  const response = await axios.get(`${API_BASE}/session/${sessionId}`);
  const data = response.data;

  return {
    ...data,
    characterDesign: safeParseCharacterDesign(data.characterDesign),
  };
};

// -----------------------------------------
// Fetch an act by number
// -----------------------------------------
export const getAct = async (actNumber: number): Promise<Act> => {
  const response = await axios.get(`${API_BASE}/act/${actNumber}`);
  let act = response.data;

  // ✅ FIX unwrap array safely
  if (act.choices && "$values" in act.choices) {
    act.choices = act.choices.$values;
  }

  return act;
};

// -----------------------------------------
// Fetch current act for a session
// -----------------------------------------
export const getCurrentAct = async (
  sessionId: number
): Promise<{ session: PlayerSessionFromApi; act: Act } | null> => {
  try {
    const res = await axios.get(`${API_BASE}/currentAct/${sessionId}`);
    let data = res.data;

    console.log("Raw getCurrentAct data:", data);

    if (!data) return null;

    // ✅ FIX ensure session.characterDesign parsed
    if (data.session) {
      data.session.characterDesign = safeParseCharacterDesign(
        data.session.characterDesignJson || data.session.characterDesign
      );
    }

    // ✅ FIX unwrap choices
    if (data.act && data.act.choices && "$values" in data.act.choices) {
      data.act.choices = data.act.choices.$values;
    }

    return data;
  } catch (error) {
    console.error("Error fetching current act:", error);
    return null;
  }
};

// -----------------------------------------
// Fetch choices for an act
// -----------------------------------------
export const getChoicesForAct = async (actId: number) => {
  const response = await axios.get(`${API_BASE}/choices/${actId}`);
  return response.data;
};


// -----------------------------------------
// Fetch updated character design
// -----------------------------------------
export async function updateCharacterDesign(sessionId: number, characterDesign: any) {
  const response = await fetch(`http://localhost:5151/api/story/updateCharacter/${sessionId}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(characterDesign),
  });

  if (!response.ok) {
    const text = await response.text();
    throw new Error(`Failed to update character design: ${text}`);
  }

  return response.json();
}

