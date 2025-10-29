

const API_BASE_URL = 'http://localhost:5151/api/story';

/**
 * Fetch the full story, including acts and choices.
 */
export async function getStory() {
  const response = await fetch(API_BASE_URL);
  if (!response.ok) {
    throw new Error(`Failed to fetch story: ${response.statusText}`);
  }
  return response.json();
}

/**
 * Fetch a specific act by its number.
 * @param number - The act number to fetch
 */
export async function getAct(number: number) {
  const response = await fetch(`${API_BASE_URL}/act/${number}`);
  if (!response.ok) {
    throw new Error(`Failed to fetch act ${number}: ${response.statusText}`);
  }
  return response.json();
}

/**
 * Fetch all choices for a specific act by actId.
 * @param actId - The ID of the act
 */
export async function getChoicesForAct(actId: number) {
  const response = await fetch(`${API_BASE_URL}/choices/${actId}`);
  if (!response.ok) {
    throw new Error(`Failed to fetch choices for act ${actId}: ${response.statusText}`);
  }
  return response.json();
}
