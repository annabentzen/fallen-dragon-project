import { Character, CharacterPose } from "../types/character";
import styles from '../styles/CharacterBuilder.module.css';

interface CharacterBuilderProps {
  character: Character;
  poses: CharacterPose[];
  onHairChange: (hair: string) => void;
  onFaceChange: (face: string) => void;
  onOutfitChange: (outfit: string) => void;
  onPoseChange: (poseId: number | null) => void;
}

export default function CharacterBuilder({
  character,
  poses,
  onHairChange,
  onFaceChange,
  onOutfitChange,
  onPoseChange
}: CharacterBuilderProps) {

  const { hair, face, outfit, poseId } = character;
  const selectedPose = poses.find(pose => pose.id === character.poseId);

  const hairOptions = ["hair1.png", "hair2.png", "hair3.png"];
  const faceOptions = ["face1.png", "face2.png", "face3.png"];
  const outfitOptions = ["clothing1.png", "clothing2.png", "clothing3.png"];

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
        
        {/* Hair arrows - TOP */}
        <div className={styles.arrowRow}>
        <button 
          onClick={() => cycleOption(hair, hairOptions, onHairChange, 'prev')} 
          className={styles.arrowButtonLeft}
          aria-label="Previous hair"
          />
          <div className={styles.spacer}></div>
          <button 
            onClick={() => cycleOption(hair, hairOptions, onHairChange, 'next')} 
            className={styles.arrowButtonRight}
            aria-label="Next hair"
            />
        </div>

        {/* Character preview */}
        <div className={styles.previewContainer}>
          {/* Base image only shows if no pose is selected*/}
          {!poseId && (
            <img
              src="/images/base.png"
              alt="base"
              className={styles.characterImage}
            />
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
          {character.poseId && selectedPose && (
            <img
              src={`/images/poses/${selectedPose.imageUrl}`}
              alt="pose"
              className={styles.characterImage}
            />
          )}

          {/* Face arrows - MIDDLE (overlaid on character) */}
          <button 
            onClick={() => cycleOption(face, faceOptions, onFaceChange, 'prev')} 
            className={`${styles.arrowButtonOverlay} ${styles.arrowLeft}`}
            aria-label="Previous face"/>
          <button 
            onClick={() => cycleOption(face, faceOptions, onFaceChange, 'next')} 
            className={`${styles.arrowButtonOverlay} ${styles.arrowRight}`}
            aria-label="Next face"/>
        </div>

        {/* Outfit arrows - BOTTOM */}
        <div className={styles.arrowRow}>
          <button 
            onClick={() => cycleOption(outfit, outfitOptions, onOutfitChange, 'prev')} 
            className={styles.arrowButtonLeft}
            aria-label="Previous outfit"/>
          <div className={styles.spacer}></div>
          <button 
            onClick={() => cycleOption(outfit, outfitOptions, onOutfitChange, 'next')} 
            className={styles.arrowButtonRight}
            aria-label="Next outfit"/>
        </div>
      </div>

      {/* Pose selector - traditional dropdown */}
      <div className={styles.poseSection}>
        <label className={styles.poseLabel}>Pose:</label>
        <select
          value={poseId || ''}
          onChange={(e) => onPoseChange(e.target.value ? Number(e.target.value) : null)}
          className={styles.select}
          aria-label="Select character pose"
        >
          <option value="">Select a pose</option>
          {poses.map(p => (
            <option key={p.id} value={p.id}>{p.name}</option>
          ))}
        </select>
      </div>
    </div>
  );
}