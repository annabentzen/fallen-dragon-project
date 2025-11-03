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
/*
export const createSession = async (sessionData: {
  characterName: string;
  characterDesign: CharacterDesign;
  storyId: number;
}): Promise<PlayerSessionFromApi> => {
  const response = await axios.post(`${API_BASE}/start`, {
    characterName: sessionData.characterName,
    characterDesign: sessionData.characterDesign, // backend expects camelCase now
    storyId: sessionData.storyId,
  });
  return response.data;
};
*/
export const createSession = async (sessionData: {
  characterName: string;
  characterDesign: CharacterDesign;
  storyId: number;
}): Promise<PlayerSessionFromApi> => {
  const response = await axios.post(`${API_BASE}/start`, {
    characterName: sessionData.characterName,
    characterDesignJson: JSON.stringify(sessionData.characterDesign),
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

  // The backend already sends camelCase + characterDesign (object)
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
export const getCurrentAct = async (sessionId: number) => {
  const response = await axios.get(`${API_BASE}/currentAct/${sessionId}`);
  return response.data; // backend returns { session, act }
};

// -----------------------------------------
// Fetch choices for an act
// -----------------------------------------
export const getChoicesForAct = async (actId: number) => {
  const response = await axios.get(`${API_BASE}/choices/${actId}`);
  return response.data;
};
