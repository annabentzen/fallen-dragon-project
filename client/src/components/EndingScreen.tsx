import React from 'react';
import styles from '../styles/Ending.module.css';
import { useNavigate } from 'react-router-dom';
import { deleteSession } from '../services/storyApi';

interface EndingScreenProps {
  onRestart: () => void;
  sessionId: number;
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

const EndingScreen: React.FC<EndingScreenProps> = ({ onRestart, sessionId, endingType, endingText }) => {
  const title = titles[endingType] ?? titles.default;
  const subtitle = subtitles[endingType] ?? subtitles.default;
  const navigate = useNavigate();

  // Debug
  console.log('EndingScreen - sessionId:', sessionId);

  const handleDeletePlaythrough = async () => {
    if (!sessionId || sessionId <= 0) {
      console.warn('Delete blocked: invalid sessionId', sessionId);
      alert('This playthrough cannot be deleted.');
      return;
    }

    if (!window.confirm('Permanently delete this playthrough?')) return;

    try {
      await deleteSession(sessionId);
      navigate('/home');
    } catch (error) {
      console.error('Failed to delete session:', error);
      alert('Failed to delete. Please try again later.');
    }
  };

  const canDelete = sessionId > 0;

return (
    <div className={`${styles.endingContainer} ${styles[`ending--${endingType}`]}`}>
      <h1 className={styles.title}>{title}</h1>
      <p className={styles.subtitle}>{subtitle}</p>
      <p className={styles.endingText}>{endingText}</p>
      
      <button onClick={onRestart} className={styles.restartButton}>
        Play Again
      </button>
      
      <button
        onClick={handleDeletePlaythrough}
        className={styles.deleteButton}
        disabled={!canDelete}
        title={canDelete ? 'Permanently delete this playthrough' : 'Nothing to delete'}
      >
        Clear This Playthrough
      </button>
    </div>
  );
};

export default EndingScreen;

