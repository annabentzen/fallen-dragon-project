import React from 'react';
import styles from '../styles/Ending.module.css';
import { NavigateFunction } from 'react-router-dom';

interface EndingScreenProps {
  onRestart: () => void;
  navigate?: NavigateFunction;  
  endingType: 'heroDeath' | 'dragonKilled' | 'tragedy' | 'ignored' | 'recovery' | 'guardian' | 'default';
  endingText: string;
}

const titles: Record<string, string> = {
  heroDeath: "Perished in Flames",
  dragonKilled: "A Merciful Death",
  tragedy: "Needless Slaughter",
  ignored: "Willful Ignorance",
  recovery: "A New Dawn",
  guardian: "The Eternal Guardian",
  default: "The End"
};

const subtitles: Record<string, string> = {
  heroDeath: "Curiosity killed the hero.",
  dragonKilled: "The dragon no longer suffers.",
  tragedy: "Fear destroyed everything.",
  ignored: "Some mysteries are best forgotten.",
  recovery: "The dragon soars free once more.",
  guardian: "A legend was born.",
  default: "Thank you for playing."
};

const EndingScreen: React.FC<EndingScreenProps> = ({ onRestart, endingType, endingText }) => {
  const title = titles[endingType] ?? titles.default;
  const subtitle = subtitles[endingType] ?? subtitles.default;

  const handleClick = () => {
    console.log("Play Again clicked!"); 
    onRestart();
  };

  return (
    <div className={`${styles.endingContainer} ${styles[`ending--${endingType}`]}`}>
      <h1 className={styles.title}>{title}</h1>
      <p className={styles.subtitle}>{subtitle}</p>
      <p className={styles.endingText}>{endingText}</p>
      <button onClick={handleClick} className={styles.restartButton}>
        Play Again
      </button>
    </div>
  );
};

export default EndingScreen;