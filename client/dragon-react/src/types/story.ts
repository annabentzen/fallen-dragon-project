export interface Choice {
  id: number;
  text: string;
  nextActId: number;
}

export interface Act {
  id: number;
  text: string;
  choices: Choice[];
}

export interface Story {
  id: number;
  title: string;
  acts: Act[];
}