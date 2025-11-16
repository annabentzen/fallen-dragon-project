import React, { useEffect, useState } from "react";
import { Act } from "../types/story";
import { getAct, getChoicesForAct, getCurrentAct, moveToNextAct } from "../services/storyApi";
import ChoiceButton from "./ChoiceButton";
import "../styles/story.css";

// Add this interface
interface StoryViewProps {
  onEnd: () => void;
}

const StoryView: React.FC<StoryViewProps> = ({ onEnd }) => {
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
  try {
    // 1. Tell backend to move to next act
    await moveToNextAct(sessionId, nextActNumber);

    // 2. Fetch updated act from backend
    const updatedAct = await getCurrentAct(sessionId);

    // 3. Update state
    setCurrentAct(updatedAct.act);
    setActNumber(updatedAct.act.ActNumber);
  } catch (error) {
    console.error("Failed to move to next act:", error);
  }
};



  if (!currentAct) return <p>Loading story...</p>;

  return (
    <div className="story-container">
      <h2>Act {currentAct.actNumber}</h2>
      <p className="story-text">{currentAct.text}</p>
      <div className="choices">
        {currentAct.choices.map(choice => (
          <ChoiceButton
            key={choice.id}
            choice={choice}
            onSelect={handleChoice}
          />
        ))}
      </div>
    </div>
  );
};

export default StoryView;
