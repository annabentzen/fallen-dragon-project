import axios from "axios";
import { Act } from "../models/story";

const API_BASE = "http://localhost:5000/api/story"; 

export const getAct = async (storyId: number, actNumber: number): Promise<Act> => {
  const response = await axios.get(`${API_BASE}/${storyId}/${actNumber}`);
  return response.data;
};

export const makeChoice = async (storyId: number, nextActNumber: number): Promise<Act> => {
  const response = await axios.post(`${API_BASE}/choose`, {
    storyId,
    nextActNumber
  });
  return response.data;
};
