import { Character, CharacterPose } from "../types/character";
import { getToken } from "./authApi";

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

const getHeaders = () => {
  const token = getToken();
  return {
    "Content-Type": "application/json",
    ...(token && { Authorization: `Bearer ${token}` }),
  };
};

export const createSession = async (characterName: string, character: Character) => {
  const response = await fetch(`${API_BASE}/api/story/start`, {
    method: "POST",
    headers: getHeaders(),
    body: JSON.stringify({ 
      characterName, 
      character: {
        head: character.head,
        body: character.body,
        poseId: character.poseId 
      },
      storyId: 1
    }),
  });
  if (!response.ok) throw new Error("Failed to create session");
  return response.json();
};

export const getCurrentAct = async (sessionId: number): Promise<ActDto> => {
  console.log("[storyApi] Loading current act for session", sessionId);
  const response = await fetch(`${API_BASE}/api/story/currentAct/${sessionId}`, {
    headers: getHeaders(),
  });

  if (!response.ok) {
    const err = await response.text();
    throw new Error(`Failed to load act: ${response.status} ${err}`);
  }

  const act: ActDto = await response.json();
  console.log("[storyApi] Act loaded:", act);
  return act;
};

export const makeChoice = async (sessionId: number, nextActNumber: number): Promise<ActDto> => {
  console.log("[storyApi] Choosing next act:", nextActNumber);

  const response = await fetch(`${API_BASE}/api/story/nextAct/${sessionId}`, {
    method: "POST",
    headers: getHeaders(),
    body: JSON.stringify({ nextActNumber }),
  });

  if (!response.ok) throw new Error("Failed to make choice");
  const act: ActDto = await response.json();
  console.log("[storyApi] New act after choice:", act);
  return act;
};

export const getCharacter = async (sessionId: number) => {
  console.log("[storyApi] Fetching character for session", sessionId);
  const response = await fetch(`${API_BASE}/api/story/${sessionId}/character`, {
    headers: getHeaders(),
  });
  if (!response.ok) throw new Error("Failed to load character");
  const data = await response.json();
  console.log("[storyApi] Character loaded", data);
  return data;
};

export const getSession = async (sessionId: number): Promise<PlayerSessionDto> => {
  const response = await fetch(`${API_BASE}/api/story/session/${sessionId}`, {
    headers: getHeaders(),
  });
  if (!response.ok) throw new Error("Failed to load session");
  return response.json();
};

export const getCharacterForSession = async (sessionId: number) => {
  return getCharacter(sessionId);
};

export const moveToNextAct = async (sessionId: number, nextActNumber: number): Promise<void> => {
  const response = await fetch(`${API_BASE}/api/story/nextAct/${sessionId}`, {
    method: "POST",
    headers: getHeaders(),
    body: JSON.stringify({ nextActNumber }),
  });
  if (!response.ok) {
    const err = await response.text();
    throw new Error(`Failed to move to act ${nextActNumber}: ${err}`);
  }
};

export const updateCharacter = async (sessionId: number, character: any): Promise<void> => {
  const response = await fetch(`${API_BASE}/api/character/session/${sessionId}`, {
    method: "PUT",
    headers: getHeaders(),
    body: JSON.stringify(character),
  });
  if (!response.ok) {
    const err = await response.text();
    throw new Error(`Failed to update character: ${err}`);
  }
};

export const deleteSession = async (sessionId: number): Promise<void> => {
  const response = await fetch(`${API_BASE}/api/PlayerSession/${sessionId}`, {
    method: 'DELETE',
    headers: getHeaders(), 
  });
  
  if (!response.ok) {
    throw new Error('Failed to delete session');
  }
};