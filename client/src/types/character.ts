export interface Character {
  id: number;
  head: string;
  body: string;
  poseId: number | null;
}

export interface CharacterPose {
  id: number;
  name: string;
  imageUrl: string;
  characterType?: string; 
}
