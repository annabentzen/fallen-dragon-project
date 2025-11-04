// src/services/storyApi.ts
import axios from "axios";
import { Act, PlayerSessionFromApi } from "../types/story";

const API_BASE = "http://localhost:5151/api/story";

// -----------------------------------------
// Interfaces (frontend representation)
// -----------------------------------------
export interface CharacterDesign {
  hair?: string;
  outfit?: string;
  color?: string;
}

// -----------------------------------------
// Create new story session
// -----------------------------------------
export const createSession = async (sessionData: {
  characterName: string;
  characterDesign: CharacterDesign;
  storyId: number;
}): Promise<PlayerSessionFromApi> => {
  const response = await axios.post(`${API_BASE}/start`, {
    characterName: sessionData.characterName,
    characterDesignJson: JSON.stringify(sessionData.characterDesign), // backend expects JSON string
    storyId: sessionData.storyId,
  });
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

  // Convert JSON string to object if needed
  return {
    ...data,
    characterDesign:
      typeof data.characterDesign === "string"
        ? JSON.parse(data.characterDesign || "{}")
        : data.characterDesign,
  };
};

// -----------------------------------------
// Fetch an act by number
// -----------------------------------------
export const getAct = async (actNumber: number): Promise<Act> => {
  const response = await axios.get(`${API_BASE}/act/${actNumber}`);
  return response.data;
};

// -----------------------------------------
// Fetch current act for a session
// -----------------------------------------
// src/services/storyApi.ts
export const getCurrentAct = async (sessionId: number): Promise<{ session: PlayerSessionFromApi; act: Act } | null> => {
  try {
    const res = await axios.get(`http://localhost:5151/api/story/currentAct/${sessionId}`);
    let data = res.data;

    console.log("Raw getCurrentAct data:", data);

    if (!data) return null;

    // Unwrap $values if needed
    if (data.act && data.act.choices && '$values' in data.act.choices) {
      data.act.choices = data.act.choices.$values;
      console.log("Unwrapped choices array:", data.act.choices);
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
