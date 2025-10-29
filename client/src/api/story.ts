import { Story } from "../types/story";

const API_URL =
  import.meta.env.VITE_API_URL || "https://localhost:5001/api/story";

export async function getStories(): Promise<Story[]> {
  const res = await fetch(API_URL);
  if (!res.ok) throw new Error("Failed to fetch stories");
  return res.json();
}

export async function getStory(id: number): Promise<Story> {
  const res = await fetch(`${API_URL}/${id}`);
  if (!res.ok) throw new Error("Failed to fetch story");
  return res.json();
}

export async function createStory(story: Story): Promise<Story> {
  const res = await fetch(API_URL, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(story),
  });
  if (!res.ok) throw new Error("Failed to create story");
  return res.json();
}

export async function updateStory(story: Story) {
  const res = await fetch(`${API_URL}/${story.id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(story),
  });
  if (!res.ok) throw new Error("Failed to update story");
}

export async function deleteStory(id: number) {
  const res = await fetch(`${API_URL}/${id}`, { method: "DELETE" });
  if (!res.ok) throw new Error("Failed to delete story");
}
