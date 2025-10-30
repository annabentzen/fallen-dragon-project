export interface Choice {
  id: number;
  text: string;
  nextActNumber: number;
}

export interface Act {
  id: number;
  actNumber: number;
  text: string;
  choices: Choice[];
}

export interface Story {
  id: number;
  title: string;
  acts: Act[];
}

export interface CharacterDesign {
  hair?: string;
  outfit?: string;
  color?: string;
}

export interface PlayerSessionFromApi {
  sessionId: number;
  characterName: string;
  characterDesign: CharacterDesign; // matches API response
  storyId: number;
  currentActNumber: number;
  isCompleted: boolean;
}

