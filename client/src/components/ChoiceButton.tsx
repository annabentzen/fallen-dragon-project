import React from "react";
import { Choice } from "../types/story";

interface Props {
  choice: Choice;
  onSelect: (nextActNumber: number) => void;
}

const ChoiceButton: React.FC<Props> = ({ choice, onSelect }) => {
  return (
    <button className="choice-btn" onClick={() => onSelect(choice.nextActNumber)}>
      {choice.text}
    </button>
  );
};

export default ChoiceButton;
