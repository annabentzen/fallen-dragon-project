import axios from 'axios';

const API_BASE = 'http://localhost:5151';

export interface Character {
    id?: number;
    hair: string;
    face: string;
    clothing: string;
    poseId: number | null;
    pose?: CharacterPose;
}

export interface CharacterPose {
    id: number;
    name: string;
    imageUrl: string;
}

// -------------------------------CRUD Operations-------------------------------

// CREATE - save character to backend
export const createCharacter = async (character: Omit<Character, 'id'>): Promise<Character> => {
  const response = await axios.post(`${API_BASE}/Character/Create`, character);
  return response.data;
};

// READ - Get character by ID
export const getCharacter = async (id: number): Promise<Character> => {
  const response = await axios.get(`${API_BASE}/Character/Result/${id}`);
  return response.data;
};

// UPDATE - Update character
export const updateCharacter = async (id: number, character: Character): Promise<Character> => {
  const response = await axios.post(`${API_BASE}/Character/UpdateCharacterPose`, {
    id: character.id,
    poseId: character.poseId,
    hair: character.hair,
    face: character.face,
    clothing: character.clothing,
  });
  return response.data;
};

// DELETE - Delete character
export const deleteCharacter = async (id: number): Promise<void> => {
  await axios.post(`${API_BASE}/Character/Delete/${id}`);
};


// -----------------------------------------
// Pose Operations
// -----------------------------------------

// Get all poses
export const getAllPoses = async (): Promise<CharacterPose[]> => {
  const response = await axios.get(`${API_BASE}/api/poses`);
  return response.data;
};

// -----------------------------------------
// LocalStorage helpers for start over feature
// -----------------------------------------

// Save character to localStorage
export const saveCharacterToLocal = (character: Character): void => {
  localStorage.setItem('lastCharacter', JSON.stringify(character));
};

// Load character from localStorage
export const loadCharacterFromLocal = (): Character | null => {
  const saved = localStorage.getItem('lastCharacter');
  if (saved) {
    return JSON.parse(saved);
  }
  return null;
};

// Clear saved character
export const clearSavedCharacter = (): void => {
  localStorage.removeItem('lastCharacter');
};