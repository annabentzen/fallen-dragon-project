import { Character, CharacterPose } from "../types/character";
import styles from "../styles/CharacterBuilder.module.css";
import { useEffect, useRef, useState } from "react";

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
  onPoseChange,
}: CharacterBuilderProps) {
  const { head, body, poseId } = character;

  const [isDropdownOpen, setIsDropdownOpen] = useState(false);
  const dropdownRef = useRef<HTMLDivElement>(null);

  // Avatar head and body options
  const headOptions = [
    "knight-head.png",
    "mage-head1.png",
    "mage2-head.png",
    "rogue-head.png",
  ];

  const bodyOptions = [
    "knight-body.png",
    "mage-body.png",
    "mage2-body.png",
    "rogue-body.png",
  ];

  // Extract character type from body filename
  const characterType = body.split("-")[0]; // "knight", "mage", or "rogue"

  // Filter poses to only show ones matching the current body type
  const availablePoses = poses.filter(
    (pose) => !pose.characterType || pose.characterType === characterType
  );

  const selectedPose = availablePoses.find((pose) => pose.id === poseId);

  // Reset pose if it's not valid for current character type
  useEffect(() => {
    if (poseId && !availablePoses.find((p) => p.id === poseId)) {
      onPoseChange(null);
    }
  }, [characterType, availablePoses, poseId, onPoseChange]);

  // Close dropdown when clicking outside
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (
        dropdownRef.current &&
        !dropdownRef.current.contains(event.target as Node)
      ) {
        setIsDropdownOpen(false);
      }
    };

    if (isDropdownOpen) {
      document.addEventListener("mousedown", handleClickOutside);
    }

    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, [isDropdownOpen]);

  const currentHead = head;
  const currentBody = body;

  const cycleOption = (
    currentValue: string,
    options: string[],
    setter: (val: string) => void,
    direction: "next" | "prev"
  ) => {
    const index = options.indexOf(currentValue);
    const newIndex =
      direction === "next"
        ? (index + 1) % options.length
        : (index - 1 + options.length) % options.length;
    setter(options[newIndex]);
  };

  const handlePoseSelect = (pose: CharacterPose | null) => {
    onPoseChange(pose ? pose.id : null);
    setIsDropdownOpen(false);
  };

  return (
    <div className={styles.container}>
      <h2 className={styles.header}>Change your look</h2>

      {/* Character preview with side arrows */}
      <div className={styles.characterSection}>
        {/* Wrapper for character and arrows */}
        <div className={styles.characterWrapper}>
          {/* Head arrows - LEFT TOP */}
          <button
            onClick={() =>
              cycleOption(currentHead, headOptions, onHeadChange, "prev")
            }
            className={`${styles.arrowButton} ${styles.arrowHeadLeft}`}
            aria-label="Previous head"
          ></button>

          {/* Head arrows - RIGHT TOP */}
          <button
            onClick={() =>
              cycleOption(currentHead, headOptions, onHeadChange, "next")
            }
            className={`${styles.arrowButton} ${styles.arrowHeadRight}`}
            aria-label="Next head"
          ></button>

          {/* Body arrows - LEFT MIDDLE */}
          <button
            onClick={() =>
              cycleOption(currentBody, bodyOptions, onBodyChange, "prev")
            }
            className={`${styles.arrowButton} ${styles.arrowBodyLeft}`}
            aria-label="Previous body"
          ></button>

          {/* Body arrows - RIGHT MIDDLE */}
          <button
            onClick={() =>
              cycleOption(currentBody, bodyOptions, onBodyChange, "next")
            }
            className={`${styles.arrowButton} ${styles.arrowBodyRight}`}
            aria-label="Next body"
          ></button>

          {/* Character preview */}
          <div className={styles.previewContainer}>
            {/* Body layer - only show if NO pose is selected */}
            {!poseId && (
              <img
                src={`/images/avatar/body/${currentBody}`}
                alt="body"
                className={`${styles.characterImage} ${
                  styles[
                    `body${
                      characterType.charAt(0).toUpperCase() +
                      characterType.slice(1)
                    }`
                  ]
                }`}
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
                className={`${styles.characterImage} ${
                  styles[
                    `pose${
                      characterType.charAt(0).toUpperCase() +
                      characterType.slice(1)
                    }`
                  ]
                } ${
                  selectedPose.name
                    ? styles[`pose${selectedPose.name.replace(/\s+/g, "")}`]
                    : ""
                }`}
              />
            )}
          </div>
        </div>
      </div>

      {/* Pose selector */}
      <div className={styles.poseSection}>
        <label className={styles.poseLabel}>Pose ({characterType}):</label>
        <div className={styles.customDropdown} ref={dropdownRef}>
          {/* Dropdown button */}
          <button
            className={styles.dropdownButton}
            onClick={() => setIsDropdownOpen(!isDropdownOpen)}
            aria-label="Select character pose"
          >
            <span>{selectedPose ? selectedPose.name : "Select a pose"}</span>
            <span className={styles.dropdownArrow}>▼</span>
          </button>

          {/* Dropdown menu */}
          {isDropdownOpen && (
            <div className={styles.dropdownMenu}>
              <button
                className={styles.dropdownOption}
                onClick={() => handlePoseSelect(null)}
              >
                Select a Pose
              </button>
              {availablePoses.map((pose) => (
                <button
                  key={pose.id}
                  className={styles.dropdownOption}
                  onClick={() => handlePoseSelect(pose)}
                >
                  {pose.name}
                </button>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
