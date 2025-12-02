import { Character, CharacterPose } from "../types/character";
import styles from '../styles/CharacterBuilder.module.css';
import { useEffect } from "react";

interface CharacterBuilderProps {
  character: Character;
  poses: CharacterPose[];
  onHeadChange: (head: string) => void;  
  onBodyChange: (body: string) => void;  
  onPoseChange: (poseId: number | null) => void;
}

export default function CharacterBuilder({
  character,
  poses,
  onHeadChange,
  onBodyChange,
  onPoseChange
}: CharacterBuilderProps) {

  const { head, body, poseId } = character;


  const headOptions = [
  "knight-head.png", 
  "mage1-head.png", 
  "mage2-head.png",  
  "rogue-head.png"
]; 

const bodyOptions = [
  "knight-body.png", 
  "mage1-body.png", 
  "mage2-body.png", 
  "rogue-body.png"
];

// Extract character type from body to filter compatible poses
  const characterType = body.split('-')[0]; 
  
  // Filter poses to only show ones matching the current body type
  const availablePoses = poses.filter(pose => 
    !pose.characterType || pose.characterType === characterType
  );
  
  const selectedPose = availablePoses.find(pose => pose.id === poseId);

// Reset pose if it's not valid for current character type
  useEffect(() => {
    if (poseId && !availablePoses.find(p => p.id === poseId)) {
      onPoseChange(null);
    }
  }, [characterType, availablePoses, poseId, onPoseChange]);

  const currentHead = head; 
  const currentBody = body; 

  const cycleOption = (
    currentValue: string,
    options: string[],
    setter: (val: string) => void,
    direction: 'next' | 'prev'
  ) => {
    const index = options.indexOf(currentValue);
    const newIndex = direction === 'next' 
      ? (index + 1) % options.length
      : (index - 1 + options.length) % options.length;
    setter(options[newIndex]);
  };

  return (
    <div className={styles.container}>
      <h3 className={styles.header}>Customize Your Character</h3>

      <div className={styles.characterSection}>
        
        {/* Head arrows */}
        <div className={styles.arrowRow}>
          <button 
            onClick={() => cycleOption(currentHead, headOptions, onHeadChange, 'prev')} 
            className={styles.arrowButtonLeft}
            aria-label="Previous head"
          >
            ◀
          </button>
          <div className={styles.spacer}></div>
          <button 
            onClick={() => cycleOption(currentHead, headOptions, onHeadChange, 'next')} 
            className={styles.arrowButtonRight}
            aria-label="Next head"
          >
            ▶
          </button>
        </div>

        <div className={styles.previewContainer}>
          {/* Body hidden when pose selected since poses replaces default body */}
          {!poseId && (
            <img 
              src={`/images/avatar/body/${currentBody}`} 
              alt="body" 
              className={`${styles.characterImage} ${styles[`body${characterType.charAt(0).toUpperCase() + characterType.slice(1)}`]}`}
            />
          )}

          <img 
            src={`/images/avatar/heads/${currentHead}`} 
            alt="head" 
            className={styles.characterImage}
          />

          {poseId && selectedPose && (
            <img
              src={`/images/avatar/poses/${selectedPose.imageUrl}`}
              alt="pose"
              className={`${styles.characterImage} ${styles[`pose${characterType.charAt(0).toUpperCase() + characterType.slice(1)}`]} ${selectedPose.name ? styles[`pose${selectedPose.name.replace(/\s+/g, '')}`] : ''}`}            />
          )}

          {/* Body arrows overlaid on character */}
          <button 
            onClick={() => cycleOption(currentBody, bodyOptions, onBodyChange, 'prev')} 
            className={`${styles.arrowButtonOverlay} ${styles.arrowLeft}`}
            aria-label="Previous body"
          >
            ◀
          </button>
          <button 
            onClick={() => cycleOption(currentBody, bodyOptions, onBodyChange, 'next')} 
            className={`${styles.arrowButtonOverlay} ${styles.arrowRight}`}
            aria-label="Next body"
          >
            ▶
          </button>
        </div>

        <div className={styles.arrowRow}>
          <div className={styles.infoText}>
            Mix and match heads and bodies!
          </div>
        </div>
      </div>

    <div className={styles.poseSection}>
      <label className={styles.poseLabel}>Pose ({characterType}):</label>
      <select
        value={poseId || ''}
        onChange={(e) => onPoseChange(e.target.value ? Number(e.target.value) : null)}
        className={styles.select}
        aria-label="Select character pose"
      >
        <option value="">Select a pose</option>
        {availablePoses.map(p => (
          <option key={p.id} value={p.id}>{p.name}</option>
        ))}
      </select>
    </div>
    </div>
  );
}