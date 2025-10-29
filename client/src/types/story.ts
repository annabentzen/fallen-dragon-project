export interface Choice {
  id: number;
  text: string;
  nextActNumber: number;
}

export interface Act {
  id: number;
  actNumber: number;
  text: string;
  choices: Choice[];
}

export interface Story {
  id: number;
  title: string;
  acts: Act[];
}



// Get a specific act by its number or ID
export async function getAct(actNumber: number) {
  const res = await fetch(`${API_URL}/act/${actNumber}`);
  if (!res.ok) throw new Error("Failed to fetch act");
  return res.json();
}

// Get choices for a given act
export async function getChoicesForAct(actId: number) {
  const res = await fetch(`${API_URL}/choices/${actId}`);
  if (!res.ok) throw new Error("Failed to fetch choices");
  return res.json();
}
