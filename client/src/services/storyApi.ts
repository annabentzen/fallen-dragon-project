// src/services/storyApi.ts
import axios from "axios";
import { Character } from "../types/character";
import { PlayerSessionFromApi } from "../types/story";

const API_BASE = "http://localhost:5151/api/story";

// Helper to log API calls
const log = (message: string, data?: any) => {
  console.log(`%c[storyApi] ${message}`, "color: #4CAF50; font-weight: bold", data || "");
};
const error = (message: string, err: any) => {
  console.error(`%c[storyApi ERROR] ${message}`, "color: #f44336; font-weight: bold", err.response?.data || err.message);
};

// Create new game session
export const createSession = async (data: {
  characterName: string;
  character: Character;
  storyId: number;
}): Promise<PlayerSessionFromApi> => {
  try {
    log("Creating new session...", data);
    const response = await axios.post(`${API_BASE}/start`, data);
    log("Session created successfully!", response.data);
    return response.data;
  } catch (err: any) {
    error("Failed to create session", err);
    throw err;
  }
};

// Get session info
export const getSession = async (sessionId: number) => {
  try {
    log(`Fetching session ${sessionId}`);
    const response = await axios.get(`${API_BASE}/session/${sessionId}`);
    return response.data;
  } catch (err: any) {
    error(`Failed to fetch session ${sessionId}`, err);
    throw err;
  }
};

// Get character for editing
export const getCharacterForSession = async (sessionId: number): Promise<Character> => {
  try {
    log(`Fetching character for session ${sessionId}`);
    const response = await axios.get<Character>(`${API_BASE}/${sessionId}/character`);
    log("Character loaded", response.data);
    return response.data;
  } catch (err: any) {
    error(`Failed to load character for session ${sessionId}`, err);
    throw err;
  }
};

// Save updated character design
export const updateCharacter = async (sessionId: number, character: Character) => {
  try {
    log(`Saving character update for session ${sessionId}`, character);
    await axios.put(`${API_BASE}/updateCharacter/${sessionId}`, character);
    log("Character saved successfully!");
  } catch (err: any) {
    error(`Failed to save character (session ${sessionId})`, err);
    throw err;
  }
};

// Load current story act + choices
export const getCurrentAct = async (sessionId: number) => {
  try {
    log(`Loading current act for session ${sessionId}`);
    const response = await axios.get(`${API_BASE}/currentAct/${sessionId}`);
    return response.data;
  } catch (err: any) {
    error(`Failed to load current act (session ${sessionId})`, err);
    throw err;
  }
};

// Choose next act
export const moveToNextAct = async (sessionId: number, nextActNumber: number) => {
  try {
    log(`Moving to act ${nextActNumber} (session ${sessionId})`);
    await axios.post(`${API_BASE}/nextAct/${sessionId}`, { nextActNumber });
    log("Successfully moved to next act");
  } catch (err: any) {
    error(`Failed to move to act ${nextActNumber}`, err);
    throw err;
  }
};