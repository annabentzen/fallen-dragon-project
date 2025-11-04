export interface Choice {
  choiceId: number;
  text: string;
  actId: number;
  nextActNumber: number;
}

export interface Act {
  actNumber: number;
  text: string;
  choices: Choice[];
}


export interface Story {
  id: number;
  title: string;
  acts: Act[];
}

// CharacterDesign stores what the user selected for their character
export interface CharacterDesign {
  hair?: string;       // Filename of hair image (e.g., "hair3.png")
  face?: string;       // Filename of face image (e.g., "face1.png")
  outfit?: string;     // Filename of clothing / outfit image (e.g., "clothing3.png")
  color?: string;      // Background / placeholder color for the character
  poseId?: number;     // Selected pose ID
}

// Individual pose object returned from the API
export interface CharacterPose {
  id: number;          // Unique ID of the pose
  name: string;        // Name of the pose (e.g., "standing", "jumping")
  imageUrl: string;    // Filename or URL of the pose image (e.g., "pose1.png")
}

export interface PlayerSessionFromApi {
  sessionId: number;
  characterName: string;
  characterDesign: CharacterDesign; // matches API response
  storyId: number;
  currentActNumber: number;
  isCompleted: boolean;
  poses?: CharacterPose[]; // Available poses for the character
}

