import { Story } from "../types/story";

const API_URL = "https://localhost:5001/api/story";

export async function getStories(): Promise<Story[]> {
  const res = await fetch(API_URL);
  return res.json();
}

export async function getStory(id: number): Promise<Story> {
  const res = await fetch(`${API_URL}/${id}`);
  return res.json();
}

export async function createStory(story: Story): Promise<Story> {
  const res = await fetch(API_URL, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(story),
  });
  return res.json();
}

export async function updateStory(story: Story) {
  await fetch(`${API_URL}/${story.id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(story),
  });
}

export async function deleteStory(id: number) {
  await fetch(`${API_URL}/${id}`, { method: "DELETE" });
}
