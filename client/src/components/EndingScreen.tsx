import React from "react";

interface EndingScreenProps {
  onEnd: () => void;
}

const EndingScreen: React.FC<EndingScreenProps> = ({ onEnd }) => {
  return (
    <div className="ending-screen">
      <h2>The End</h2>
      <button onClick={onEnd}>Restart Story</button>
    </div>
  );
};

export default EndingScreen;
