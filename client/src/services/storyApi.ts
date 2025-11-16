
import axios from "axios";
import { Act, PlayerSessionFromApi, Choice } from "../types/story";
import { Character } from "../types/character";

const API_BASE = import.meta.env.VITE_API_URL || "http://localhost:5151/api/story";

// -----------------------------------------
// Sessions & Acts
// -----------------------------------------
export const createSession = async (data: {
  characterName: string;
  character: Character; // now using entity
  storyId: number;
}): Promise<PlayerSessionFromApi> => {
  const res = await axios.post(`${API_BASE}/start`, data);
  return res.data; // res.data.character is full Character object
};

export const getSession = async (sessionId: number): Promise<PlayerSessionFromApi> => {
  const res = await axios.get(`${API_BASE}/session/${sessionId}`);
  return res.data; // res.data.character is Character entity
};

// -----------------------------------------
// Character for a session
// -----------------------------------------
export const getCharacterForSession = async (sessionId: number): Promise<Character> => {
  try {
    console.log(`Calling API for character of session ${sessionId}`);
    const res = await axios.get<Character>(`${API_BASE}/${sessionId}/character`);
    console.log(`Character received:`, res.data);
    return res.data;
  } catch (err) {
    console.error(`Failed to fetch character for session ${sessionId}`, err);
    throw err;
  }
};



export const updateCharacter = async (
  sessionId: number,
  character: Character
) => {
  const res = await axios.put(`${API_BASE}/updateCharacter/${sessionId}`, character);
  return res.data;
};

// -----------------------------------------
// Acts / Choices
// -----------------------------------------
export const getCurrentAct = async (
  sessionId: number
): Promise<{ session: PlayerSessionFromApi; act: Act } | null> => {
  try {
    const res = await axios.get(`${API_BASE}/currentAct/${sessionId}`);
    return res.data; // session.character is Character entity
  } catch (error) {
    console.error("Error fetching current act:", error);
    return null;
  }
};

export const moveToNextAct = async (sessionId: number, nextActNumber: number) => {
  const res = await axios.post(`${API_BASE}/nextAct/${sessionId}`, { nextActNumber });
  return res.data;
};

export const getChoicesForAct = async (actId: number): Promise<Choice[]> => {
  const res = await axios.get(`${API_BASE}/choices/${actId}`);
  return res.data;
};
