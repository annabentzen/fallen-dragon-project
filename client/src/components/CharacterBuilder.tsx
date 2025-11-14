import { CharacterPose } from '../services/characterApi';
import styles from '../styles/CharacterBuilder.module.css';

interface CharacterBuilderProps {
  hair: string;
  face: string;
  outfit: string;
  poseId: number | null;
  poses: CharacterPose[];
  onHairChange: (hair: string) => void;
  onFaceChange: (face: string) => void;
  onOutfitChange: (outfit: string) => void;
  onPoseChange: (poseId: number | null) => void;
}

const hairOptions = ['hair1.png', 'hair2.png', 'hair3.png'];
const faceOptions = ['face1.png', 'face2.png', 'face3.png'];
const outfitOptions = ['clothing1.png', 'clothing2.png', 'clothing3.png'];

export default function CharacterBuilder({
  hair,
  face,
  outfit,
  poseId,
  poses,
  onHairChange,
  onFaceChange,
  onOutfitChange,
  onPoseChange,
}: CharacterBuilderProps) {

  const cycleOption = (
    currentValue: string,
    options: string[],
    onChange: (value: string) => void,
    direction: 'next' | 'prev'
  ) => {
    const currentIndex = options.indexOf(currentValue);
    let newIndex = direction === 'next'
      ? (currentIndex + 1) % options.length
      : (currentIndex - 1 + options.length) % options.length;
    onChange(options[newIndex]);
  };

  // Find the selected pose object
  const selectedPose = poses.find(p => p.id === poseId);

  console.log('Rendering CharacterBuilder, poses:', poses, 'selectedPose:', selectedPose);

  return (
    <div className={styles.container}>
      <h3 className={styles.header}>Customize Your Character</h3>

      {/* Character preview */}
      <div className={styles.previewContainer}>
  {/* Base image only shows if no pose is selected */}
  {!poseId && (
    <img src="/images/base.png" alt="base" className={styles.characterImage} />
  )}

  {/* Always show hair, face, and outfit */}
   <img 
          src={`/images/hair/${hair}`} 
          alt="hair" 
          className={styles.characterImage}
        />
        <img 
          src={`/images/faces/${face}`} 
          alt="face" 
          className={styles.characterImage}
        />
        <img 
          src={`/images/clothes/${outfit}`} 
          alt="clothing" 
          className={styles.characterImage}
        />


  {/* Pose image overlays everything if selected */}
      {poseId && selectedPose && (
          <img
            src={`/images/poses/${selectedPose.imageUrl}`}
            alt="pose"
            className={styles.characterImage}
          />
        )}
      </div>



      {/* Hair selector */}
         <div className={styles.selectorRow}>
        <label className={`${styles.label} ${styles.labelHair}`}>Hair:</label>
        <div className={styles.controls}>
          <button 
            onClick={() => cycleOption(hair, hairOptions, onHairChange, 'prev')} 
            className={styles.arrowButton}
            aria-label="Previous hair"
          >
            ◀
          </button>
          <span className={styles.currentValue}>{hair}</span>
          <button 
            onClick={() => cycleOption(hair, hairOptions, onHairChange, 'next')} 
            className={styles.arrowButton}
            aria-label="Next hair"
          >
            ▶
          </button>
        </div>
      </div>

      {/* Face selector */}
         <div className={styles.selectorRow}>
        <label className={`${styles.label} ${styles.labelFace}`}>Face:</label>
        <div className={styles.controls}>
          <button 
            onClick={() => cycleOption(face, faceOptions, onFaceChange, 'prev')} 
            className={styles.arrowButton}
            aria-label="Previous face"
          >
            ◀
          </button>
          <span className={styles.currentValue}>{face}</span>
          <button 
            onClick={() => cycleOption(face, faceOptions, onFaceChange, 'next')} 
            className={styles.arrowButton}
            aria-label="Next face"
          >
            ▶
          </button>
        </div>
      </div>

      {/* Clothing selector */}
        <div className={styles.selectorRow}>
        <label className={`${styles.label} ${styles.labelOutfit}`}>Outfit:</label>
        <div className={styles.controls}>
          <button 
            onClick={() => cycleOption(outfit, outfitOptions, onOutfitChange, 'prev')} 
            className={styles.arrowButton}
            aria-label="Previous outfit"
          >
            ◀
          </button>
          <span className={styles.currentValue}>{outfit}</span>
          <button 
            onClick={() => cycleOption(outfit, outfitOptions, onOutfitChange, 'next')} 
            className={styles.arrowButton}
            aria-label="Next outfit"
          >
            ▶
          </button>
        </div>
      </div>

      {/* Pose selector */}
     <div className={styles.selectorRow}>
        <label className={`${styles.label} ${styles.labelPose}`}>Pose:</label>
        <select
          value={poseId || ''}
          onChange={(e) => onPoseChange(e.target.value ? Number(e.target.value) : null)}
          className={styles.select}
          aria-label="Select character pose"
        >
          <option value="">Select a pose</option>
          {Array.isArray(poses) && poses.map(pose => (
            <option key={pose.id} value={pose.id}>{pose.name}</option>
          ))}
        </select>
      </div>
    </div>
  );
}
