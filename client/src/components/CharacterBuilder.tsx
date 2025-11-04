import { CharacterPose } from '../services/characterApi';

// define props for CharacterBuilder
interface CharacterBuilderProps {
  hair: string;
  face: string;
  clothing: string;
  poseId: number | null;
  poses: CharacterPose[];
  onHairChange: (hair: string) => void;
  onFaceChange: (face: string) => void;
  onClothingChange: (clothing: string) => void;
  onPoseChange: (poseId: number | null) => void;
}

// available options for hair, face, and clothing
const hairOptions = ['hair1.png', 'hair2.png', 'hair3.png'];
const faceOptions = ['face1.png', 'face2.png', 'face3.png'];
const clothingOptions = ['clothing1.png', 'clothing2.png', 'clothing3.png'];

export default function CharacterBuilder({
  hair,
  face,
  clothing,
  poseId,
  poses,
  onHairChange,
  onFaceChange,
  onClothingChange,
  onPoseChange,
}: CharacterBuilderProps) {
  
    // helper to cycle through options
  const cycleOption = (
    currentValue: string,
    options: string[],
    onChange: (value: string) => void,
    direction: 'next' | 'prev'
  ) => {
    // find current index in options array
    const currentIndex = options.indexOf(currentValue);
    let newIndex;
    
    if (direction === 'next') {
        // wrap around to start if at end
      newIndex = (currentIndex + 1) % options.length;
    } else {
        // wrap around to end if at start
      newIndex = (currentIndex - 1 + options.length) % options.length;
    }
    // call the parent's onChange function with the new value
    onChange(options[newIndex]);
  };

  return (
    <div style={{ padding: '20px', border: '1px solid #ccc', borderRadius: '8px', backgroundColor: '#fafafa' }}>
      <h3 style={{ marginTop: 0 }}>Customize Your Character</h3>
      
      {/* Character preview - layered images */}
      <div style={{ 
        width: '200px', 
        height: '200px', 
        position: 'relative', 
        margin: '20px auto',
        border: '2px solid #333',
        backgroundColor: '#fff'
      }}>
{/* Base layer - background/body */}
        <img 
          src="/images/base.png" 
          alt="base" 
          style={{ position: 'absolute', width: '100%', height: '100%', objectFit: 'contain' }} 
        />
        {/* Hair layer - positioned absolutely to stack on top of base */}
        <img 
          src={`/images/hair/${hair}`} 
          alt="hair" 
          style={{ position: 'absolute', width: '100%', height: '100%', objectFit: 'contain' }} 
        />
        {/* Face layer */}
        <img 
          src={`/images/faces/${face}`} 
          alt="face" 
          style={{ position: 'absolute', width: '100%', height: '100%', objectFit: 'contain' }} 
        />
        {/* Clothing layer - top layer */}
        <img 
          src={`/images/clothes/${clothing}`} 
          alt="clothing" 
          style={{ position: 'absolute', width: '100%', height: '100%', objectFit: 'contain' }} 
        />
      </div>

      {/* Hair selector */}
      <div style={{ marginBottom: '15px', display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
        <label style={{ fontWeight: 'bold', minWidth: '80px' }}>Hair:</label>
        <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
          <button 
            onClick={() => cycleOption(hair, hairOptions, onHairChange, 'prev')}
            style={{ padding: '5px 12px', cursor: 'pointer' }}
          >
            ◀
          </button>
          <span style={{ minWidth: '100px', textAlign: 'center' }}>{hair}</span>
          <button 
            onClick={() => cycleOption(hair, hairOptions, onHairChange, 'next')}
            style={{ padding: '5px 12px', cursor: 'pointer' }}
          >
            ▶
          </button>
        </div>
      </div>

      {/* Face selector */}
      <div style={{ marginBottom: '15px', display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
        <label style={{ fontWeight: 'bold', minWidth: '80px' }}>Face:</label>
        <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
          <button 
            onClick={() => cycleOption(face, faceOptions, onFaceChange, 'prev')}
            style={{ padding: '5px 12px', cursor: 'pointer' }}
          >
            ◀
          </button>
          <span style={{ minWidth: '100px', textAlign: 'center' }}>{face}</span>
          <button 
            onClick={() => cycleOption(face, faceOptions, onFaceChange, 'next')}
            style={{ padding: '5px 12px', cursor: 'pointer' }}
          >
            ▶
          </button>
        </div>
      </div>

      {/* Clothing selector */}
      <div style={{ marginBottom: '15px', display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
        <label style={{ fontWeight: 'bold', minWidth: '80px' }}>Clothing:</label>
        <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
          <button 
            onClick={() => cycleOption(clothing, clothingOptions, onClothingChange, 'prev')}
            style={{ padding: '5px 12px', cursor: 'pointer' }}
          >
            ◀
          </button>
          <span style={{ minWidth: '100px', textAlign: 'center' }}>{clothing}</span>
          <button 
            onClick={() => cycleOption(clothing, clothingOptions, onClothingChange, 'next')}
            style={{ padding: '5px 12px', cursor: 'pointer' }}
          >
            ▶
          </button>
        </div>
      </div>

      {/* Pose selector */}
      <div style={{ marginBottom: '15px', display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
        <label style={{ fontWeight: 'bold', minWidth: '80px' }}>Pose:</label>
        <select 
          value={poseId || ''} 
          onChange={(e) => onPoseChange(e.target.value ? Number(e.target.value) : null)}
          style={{ 
            flex: 1,
            marginLeft: '10px', 
            padding: '8px',
            fontSize: '14px',
            cursor: 'pointer'
          }}
        >
          <option value="">Select a pose</option>
          {poses.map(pose => (
            <option key={pose.id} value={pose.id}>
              {pose.name}
            </option>
          ))}
        </select>
      </div>
    </div>
  );
}