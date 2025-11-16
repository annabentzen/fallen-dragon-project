// src/services/characterApi.ts
import axios from "axios";
import { CharacterPose } from "../types/character";

const POSE_API = "http://localhost:5151/api/poses";

// Fetch available poses
export async function getAllPoses(): Promise<CharacterPose[]> {
  try {
    const response = await axios.get(POSE_API);
    if (Array.isArray(response.data)) return response.data;
    if (response.data && Array.isArray(response.data.poses)) return response.data.poses;
    return [];
  } catch (err) {
    console.error("getAllPoses() failed:", err);
    return [];
  }
}
