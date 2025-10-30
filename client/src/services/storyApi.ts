// src/services/storyApi.ts
import axios from 'axios';

const API_BASE_URL = 'http://localhost:5151/api/story';

export interface CharacterDesign {
  hair: string;
  outfit: string;
  color: string;
}

export interface PlayerSession {
  id: number;
  characterName: string;
  characterDesign: CharacterDesign;
  storyId: number;
}

// Create a new story session
/*
export const createSession = async (payload: {
  characterName: string;
  characterDesign: CharacterDesign;
  storyId: number;
}): Promise<PlayerSession> => {
  const response = await axios.post(`${API_BASE_URL}/create-session`, payload);
  return response.data;
};
*/

export const createSession = async (sessionData: {
  characterName: string;
  characterDesign: any;
  storyId: number;
}) => {
  const response = await axios.post(
    "http://localhost:5151/api/story/start", // <-- updated
    {
      CharacterName: sessionData.characterName,
      CharacterDesignJson: JSON.stringify(sessionData.characterDesign),
      StoryId: sessionData.storyId,
    }
  );
  return response.data;
};




// Fetch an existing session by ID
export const getSession = async (sessionId: number): Promise<PlayerSession> => {
  const response = await axios.get(`${API_BASE_URL}/session/${sessionId}`);
  return response.data;
};

// Fetch an act by number
export const getAct = async (actNumber: number) => {
  const response = await axios.get(`${API_BASE_URL}/act/${actNumber}`);
  return response.data;
};

export const getCurrentAct = async (sessionId: number) => {
  const response = await axios.get(`http://localhost:5151/api/story/currentAct/${sessionId}`);
  return response.data.act; // backend returns { session, act }
};


// Fetch choices for an act
export const getChoicesForAct = async (actId: number) => {
  const response = await axios.get(`${API_BASE_URL}/choices/${actId}`);
  return response.data;
};
