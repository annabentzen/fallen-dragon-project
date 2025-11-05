// src/services/storyApi.ts
import axios from "axios";
import { Act, PlayerSessionFromApi } from "../types/story";

const API_BASE = "http://localhost:5151/api/story";

// -----------------------------------------
// Interfaces (frontend representation)
// -----------------------------------------
export interface CharacterDesign {
  hair?: string;
  face?: string;
  outfit?: string;
  color?: string;
  poseId?: number;
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
  return {
    ...response.data,
    characterDesign:
      typeof response.data.characterDesign === "string"
        ? JSON.parse(response.data.characterDesign || "{}")
        : response.data.characterDesign,
  };
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
  let act = response.data;

  // Unwrap choices if backend returns $values
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

    // Unwrap choices if backend returns $values
    if (data.act && data.act.choices && "$values" in data.act.choices) {
      data.act.choices = data.act.choices.$values;
      console.log("Unwrapped choices array:", data.act.choices);
    }

    // Ensure characterDesign is parsed as object
    if (data.session && typeof data.session.characterDesign === "string") {
      data.session.characterDesign = JSON.parse(data.session.characterDesign || "{}");
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
