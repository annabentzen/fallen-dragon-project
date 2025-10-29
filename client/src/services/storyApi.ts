import { Story, Act, Choice } from "../types/story";

const API_BASE = "http://localhost:5000/api/story";

// Fetch a specific act
export const getAct = async (storyId: number, actNumber: number): Promise<Act> => {
  const res = await fetch(`${API_BASE}/${storyId}/acts/${actNumber}`);
  if (!res.ok) throw new Error("Failed to fetch act");
  return res.json();
};

// Make a choice and get the next act
export const makeChoice = async (storyId: number, nextActNumber: number): Promise<Act> => {
  const res = await fetch(`${API_BASE}/${storyId}/acts/${nextActNumber}/choose`, {
    method: "POST",
  });
  if (!res.ok) throw new Error("Failed to make choice");
  return res.json();
};

// Get all stories
export const getStories = async (): Promise<Story[]> => {
  const res = await fetch(API_BASE);
  if (!res.ok) throw new Error("Failed to fetch stories");
  return res.json();
};

// Create a new story
export const createStory = async (story: Story): Promise<Story> => {
  const res = await fetch(API_BASE, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(story),
  });
  if (!res.ok) throw new Error("Failed to create story");
  return res.json();
};

// Update a story
export const updateStory = async (story: Story) => {
  const res = await fetch(`${API_BASE}/${story.id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(story),
  });
  if (!res.ok) throw new Error("Failed to update story");
};

// Delete a story
export const deleteStory = async (id: number) => {
  const res = await fetch(`${API_BASE}/${id}`, { method: "DELETE" });
  if (!res.ok) throw new Error("Failed to delete story");
};
