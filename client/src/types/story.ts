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



