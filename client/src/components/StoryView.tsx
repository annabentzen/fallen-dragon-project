import React, { useEffect, useState } from "react";
import { Act } from "../models/types";
import { getAct, makeChoice } from "../services/storyApi";
import ChoiceButton from "./ChoiceButton";
import "../styles/story.css";

const StoryView: React.FC = () => {
  const [currentAct, setCurrentAct] = useState<Act | null>(null);
  const storyId = 1; // assuming single story for now
  const [actNumber, setActNumber] = useState(1);

  useEffect(() => {
    const fetchAct = async () => {
      const act = await getAct(storyId, actNumber);
      setCurrentAct(act);
    };
    fetchAct();
  }, [actNumber]);

  const handleChoice = async (nextActNumber: number) => {
    const nextAct = await makeChoice(storyId, nextActNumber);
    setCurrentAct(nextAct);
    setActNumber(nextAct.actNumber);
  };

  if (!currentAct) return <p>Loading story...</p>;

  return (
    <div className="story-container">
      <h2>Act {currentAct.actNumber}</h2>
      <p className="story-text">{currentAct.text}</p>
      <div className="choices">
        {currentAct.choices.map(choice => (
          <ChoiceButton key={choice.id} choice={choice} onSelect={handleChoice} />
        ))}
      </div>
    </div>
  );
};

export default StoryView;
