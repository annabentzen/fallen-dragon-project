// src/components/EndingScreen.tsx
import React from 'react';

interface EndingScreenProps {
  onRestart?: () => void; // optional callback to restart the story
}

const EndingScreen: React.FC<EndingScreenProps> = ({ onRestart }) => {
  const handleRestart = () => {
    if (onRestart) {
      onRestart(); // call the callback if provided
    } else {
      // default: reload the page to restart the story
      window.location.reload();
    }
  };

  return (
    <div className="ending-screen">
      <h1>The End</h1>
      <p>Thank you for playing the story!</p>
      <button onClick={handleRestart}>Restart Story</button>
    </div>
  );
};

export default EndingScreen;
