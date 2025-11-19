
export interface Story {
  id: number;
  title: string;
  acts: Act[];
}

// API response model for active player session
export interface PlayerSessionFromApi {
  sessionId: number;
  characterName: string;
  characterId: number;
  hair?: string;
  face?: string;
  outfit?: string;
  poseId?: number;
  storyId: number;
  currentActNumber: number;
  isCompleted: boolean;
}

export interface ChoiceDto {
  Text: string;
  NextActNumber: number;
}

export interface ActDto {
  ActNumber: number;
  Text: string;
  Choices: ChoiceDto[];
  IsEnding: boolean;
}

export interface Act {
  actNumber: number;
  text: string;
  choices: Choice[];
  isEnding: boolean;
}

export interface Choice {
  choiceId: number;
  text: string;
  nextActNumber: number;
}