import { Character, CharacterPose } from "../types/character";

const API_BASE = "http://localhost:5151"; 

export interface ChoiceDto {
  text: string;          
  nextActNumber: number;
}

export interface ActDto {
  actNumber: number;     
  text: string;         
  choices: ChoiceDto[];  
  isEnding: boolean;     
}

export interface PlayerSessionDto {
  sessionId: number;
  characterName: string;
  characterId: number;
  storyId: number;
  currentActNumber: number;
  isCompleted: boolean;
}

// Create new game session
export const createSession = async (characterName: string, character: Character) => {
  const response = await fetch(`${API_BASE}/api/story/start`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ characterName, character: {
        hair: character.hair,
        face: character.face,
        outfit: character.outfit,
        poseId: character.poseId 
      },
      storyId: 1
    }),
  });
  if (!response.ok) throw new Error("Failed to create session");
  return response.json();
};

// Load current act (backend now returns ActDto directly!)
export const getCurrentAct = async (sessionId: number): Promise<ActDto> => {
  console.log("[storyApi] Loading current act for session", sessionId);
  const response = await fetch(`${API_BASE}/api/story/currentAct/${sessionId}`);

  if (!response.ok) {
    const err = await response.text();
    throw new Error(`Failed to load act: ${response.status} ${err}`);
  }

  const act: ActDto = await response.json();
  console.log("[storyApi] Act loaded:", act);
  return act;
};

// Make a choice → go to next act
export const makeChoice = async (sessionId: number, nextActNumber: number): Promise<ActDto> => {
  console.log("[storyApi] Choosing next act:", nextActNumber);

  const response = await fetch(`${API_BASE}/api/story/nextAct/${sessionId}`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ nextActNumber }),
  });

  if (!response.ok) throw new Error("Failed to make choice");
  const act: ActDto = await response.json();
  console.log("[storyApi] New act after choice:", act);
  return act;
};

// Get character (for display on story page)
export const getCharacter = async (sessionId: number) => {
  console.log("[storyApi] Fetching character for session", sessionId);
  const response = await fetch(`${API_BASE}/api/story/${sessionId}/character`);
  if (!response.ok) throw new Error("Failed to load character");
  const data = await response.json();
  console.log("[storyApi] Character loaded", data);
  return data;
};

// src/api/storyApi.ts  ← ADD THESE FUNCTIONS AT THE END OF THE FILE

// Load the full session object (used for character name, etc.)
export const getSession = async (sessionId: number): Promise<PlayerSessionDto> => {
  const response = await fetch(`${API_BASE}/api/story/session/${sessionId}`);
  if (!response.ok) throw new Error("Failed to load session");
  return response.json();
};

// Legacy alias — some places still use this name
export const getCharacterForSession = async (sessionId: number) => {
  return getCharacter(sessionId); // just forward to the main function
};

// Move to next act when a choice is made
export const moveToNextAct = async (sessionId: number, nextActNumber: number): Promise<void> => {
  const response = await fetch(`${API_BASE}/api/story/nextAct/${sessionId}`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ nextActNumber }),
  });
  if (!response.ok) {
    const err = await response.text();
    throw new Error(`Failed to move to act ${nextActNumber}: ${err}`);
  }
  // No body expected — just confirm success
};

// Update character appearance during the game (edit modal)
export const updateCharacter = async (sessionId: number, character: any): Promise<void> => {
  const response = await fetch(`${API_BASE}/api/character/session/${sessionId}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(character),
  });
  if (!response.ok) {
    const err = await response.text();
    throw new Error(`Failed to update character: ${err}`);
  }
};