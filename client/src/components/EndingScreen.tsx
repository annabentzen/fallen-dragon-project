// src/components/EndingScreen.tsx
import React from 'react';

interface EndingScreenProps {
  onRestart: () => void;
}

const EndingScreen: React.FC<EndingScreenProps> = ({ onRestart }) => {
  return (
    <div
      className="ending-screen"
      style={{
        textAlign: "center",
        paddingTop: "50px",
        minHeight: "100vh",
        backgroundColor: "#ffe4e1", // Unique color for ending
      }}
    >
      <h2>The End</h2>
      <button
        onClick={onRestart}
        style={{
          padding: "10px 20px",
          fontSize: "16px",
          cursor: "pointer",
          marginTop: "20px",
        }}
      >
        Restart
      </button>
    </div>
  );
};

export default EndingScreen;
