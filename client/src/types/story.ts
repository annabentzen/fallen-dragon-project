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
  poseId?: number;     // Selected pose ID
}

// Individual pose object returned from the API
export interface CharacterPose {
  id: number;          // Unique ID of the pose
  name: string;        // Name of the pose (e.g., "standing", "jumping")
  imageUrl: string;    // Filename or URL of the pose image (e.g., "pose1.png")
}

// API response model for active player session
export interface PlayerSessionFromApi {
  sessionId: number;
  characterName: string;
  characterDesign: CharacterDesign | string;   // sometimes the backend returns this as an object, sometimes as a JSON string --> thus union typed
  characterDesignJson?: string;   // some older endpoints may still send this separately
  storyId: number;
  currentActNumber: number;
  isCompleted: boolean;
  poses?: CharacterPose[]; // optional list of available poses for the session
}
