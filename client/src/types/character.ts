export interface Character {
  id: number;
  hair: string;
  face: string;
  outfit: string;
  poseId: number | null;
}

export interface CharacterPose {
  id: number;
  name: string;
  imageUrl: string;
}
