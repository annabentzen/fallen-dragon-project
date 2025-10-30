// src/services/storyApi.ts
const API_URL = 'http://localhost:5151/api/story';

/**
 * Ask backend to create a new story session.
 * Backend returns { sessionId: "uuid" }.
 */
export async function startStory() {
  const response = await fetch(`${API_URL}/start`, {
    method: 'POST',
  });

  if (!response.ok) {
    throw new Error('Failed to start story session');
  }

  return response.json();
}

/**
 * Fetch one specific act (by actNumber)
 */
export async function getAct(actNumber: number) {
  const response = await fetch(`${API_URL}/act/${actNumber}`);
  if (!response.ok) {
    throw new Error(`Failed to fetch act ${actNumber}: ${response.statusText}`);
  }
  return response.json();
}

/**
 * Fetch all choices for a given actId
 */
export async function getChoicesForAct(actId: number) {
  const response = await fetch(`${API_URL}/choices/${actId}`);
  if (!response.ok) {
    throw new Error(`Failed to fetch choices for act ${actId}`);
  }
  return response.json();
}

export async function getCurrentActForSession(sessionId: string) {
  const response = await fetch(`${API_URL}/${sessionId}/current`);
  if (!response.ok) throw new Error('Failed to get current act for session');
  return response.json();
}

export async function updateSessionProgress(sessionId: string, nextActNumber: number) {
  const response = await fetch(`${API_URL}/${sessionId}/progress/${nextActNumber}`, {
    method: 'POST'
  });
  if (!response.ok) throw new Error('Failed to update progress');
  return response.json();
}
