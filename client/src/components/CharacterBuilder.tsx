import { Character, CharacterPose } from "../types/character";
import styles from '../styles/CharacterBuilder.module.css';
import { useEffect } from "react";

interface CharacterBuilderProps {
  character: Character;
  poses: CharacterPose[];
  onHeadChange: (head: string) => void;  // renamed from onHairChange
  onBodyChange: (body: string) => void;  // renamed from onOutfitChange
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
  //const selectedPose = poses.find(pose => pose.id === character.poseId);

  // NEW: Avatar head and body options
  const headOptions = ["mage-head1.png", "knight-head.png", "rogue-head.png"]; 
  const bodyOptions = ["knight-body.png", "mage-body.png", "rogue-body.png"];

// Extract character type from body filename (e.g., "knight-body.png" -> "knight")
  const characterType = body.split('-')[0]; // "knight", "mage", or "rogue"
  
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

     {/* Character preview with side arrows */}
      <div className={styles.characterSection}>
        
        {/* Head arrows - TOP */}
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

        {/* Character preview */}
        <div className={styles.previewContainer}>
          {/* Body layer - only show if NO pose is selected */}
          {!poseId && (
            <img 
              src={`/images/avatar/body/${currentBody}`} 
              alt="body" 
              className={styles.characterImage}
            />
          )}

          {/* Head layer - on top of body */}
          <img 
            src={`/images/avatar/heads/${currentHead}`} 
            alt="head" 
            className={styles.characterImage}
          />

          {/* Pose image replaces body when selected */}
          {poseId && selectedPose && (
            <img
              src={`/images/avatar/poses/${selectedPose.imageUrl}`}
              alt="pose"
              className={`${styles.characterImage} ${styles.poseImage} ${styles[`pose${characterType.charAt(0).toUpperCase() + characterType.slice(1)}`]}`}
            />
          )}

          {/* Body arrows - MIDDLE (overlaid on character) */}
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

        {/* Info text - BOTTOM */}
        <div className={styles.arrowRow}>
          <div className={styles.infoText}>
            Mix and match heads and bodies!
          </div>
        </div>
      </div>

      {/* Pose selector */}
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