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

  const headOptions = [
    "knight-head.png",
    "mage1-head.png",
    "mage2-head.png",
    "rogue-head.png",
  ];

  const bodyOptions = [
    "knight-body.png",
    "mage1-body.png",
    "mage2-body.png",
    "rogue-body.png",
  ];

  const characterType = body.split("-")[0];

  const availablePoses = poses.filter(
    (pose) => !pose.characterType || pose.characterType === characterType
  );

  const selectedPose = availablePoses.find((pose) => pose.id === poseId);

  useEffect(() => {
    if (poseId && !availablePoses.find((p) => p.id === poseId)) {
      onPoseChange(null);
    }
  }, [characterType, availablePoses, poseId, onPoseChange]);

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
      <div className={styles.characterSection}>
        <div className={styles.characterWrapper}>
          <button
            onClick={() =>
              cycleOption(currentHead, headOptions, onHeadChange, "prev")
            }
            className={`${styles.arrowButton} ${styles.arrowHeadLeft}`}
            aria-label="Previous head"
          ></button>

          <button
            onClick={() =>
              cycleOption(currentHead, headOptions, onHeadChange, "next")
            }
            className={`${styles.arrowButton} ${styles.arrowHeadRight}`}
            aria-label="Next head"
          ></button>

          <button
            onClick={() =>
              cycleOption(currentBody, bodyOptions, onBodyChange, "prev")
            }
            className={`${styles.arrowButton} ${styles.arrowBodyLeft}`}
            aria-label="Previous body"
          ></button>

          <button
            onClick={() =>
              cycleOption(currentBody, bodyOptions, onBodyChange, "next")
            }
            className={`${styles.arrowButton} ${styles.arrowBodyRight}`}
            aria-label="Next body"
          ></button>

          <div className={styles.previewContainer}>
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

            <img
              src={`/images/avatar/heads/${currentHead}`}
              alt="head"
              className={styles.characterImage}
            />

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

      <div className={styles.poseSection}>
        <div className={styles.customDropdown} ref={dropdownRef}>
          <button
            className={styles.dropdownButton}
            onClick={() => setIsDropdownOpen(!isDropdownOpen)}
            aria-label="Select character pose"
          >
            <span>{selectedPose ? selectedPose.name : "Standing pose"}</span>
            <span className={styles.dropdownArrow}></span>
          </button>

          {isDropdownOpen && (
            <div className={styles.dropdownMenu}>
              <button
                className={styles.dropdownOption}
                onClick={() => handlePoseSelect(null)}
              >
                Standing pose
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
