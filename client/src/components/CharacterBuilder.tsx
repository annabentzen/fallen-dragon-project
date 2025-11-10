import { CharacterPose } from '../services/characterApi';

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

  console.log('Rendering CharacterBuilder, poses:', poses);

  return (
    <div style={{ padding: '20px', border: '1px solid #ccc', borderRadius: '8px', backgroundColor: '#fafafa' }}>
      <h3 style={{ marginTop: 0 }}>Customize Your Character</h3>

      <div style={{ width: '200px', height: '200px', position: 'relative', margin: '20px auto', border: '2px solid #333', backgroundColor: '#fff' }}>
        <img src="/images/base.png" alt="base" style={{ position: 'absolute', width: '100%', height: '100%', objectFit: 'contain' }} />
        <img src={`/images/hair/${hair}`} alt="hair" style={{ position: 'absolute', width: '100%', height: '100%', objectFit: 'contain' }} />
        <img src={`/images/faces/${face}`} alt="face" style={{ position: 'absolute', width: '100%', height: '100%', objectFit: 'contain' }} />
        <img src={`/images/clothes/${outfit}`} alt="clothing" style={{ position: 'absolute', width: '100%', height: '100%', objectFit: 'contain' }} />
      </div>

      {/* Hair selector */}
      <div style={{ marginBottom: '15px', display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
        <label style={{ fontWeight: 'bold', minWidth: '80px' }}>Hair:</label>
        <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
          <button onClick={() => cycleOption(hair, hairOptions, onHairChange, 'prev')} style={{ padding: '5px 12px', cursor: 'pointer' }}>◀</button>
          <span style={{ minWidth: '100px', textAlign: 'center' }}>{hair}</span>
          <button onClick={() => cycleOption(hair, hairOptions, onHairChange, 'next')} style={{ padding: '5px 12px', cursor: 'pointer' }}>▶</button>
        </div>
      </div>

      {/* Face selector */}
      <div style={{ marginBottom: '15px', display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
        <label style={{ fontWeight: 'bold', minWidth: '80px' }}>Face:</label>
        <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
          <button onClick={() => cycleOption(face, faceOptions, onFaceChange, 'prev')} style={{ padding: '5px 12px', cursor: 'pointer' }}>◀</button>
          <span style={{ minWidth: '100px', textAlign: 'center' }}>{face}</span>
          <button onClick={() => cycleOption(face, faceOptions, onFaceChange, 'next')} style={{ padding: '5px 12px', cursor: 'pointer' }}>▶</button>
        </div>
      </div>

      {/* Clothing selector */}
      <div style={{ marginBottom: '15px', display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
        <label style={{ fontWeight: 'bold', minWidth: '80px' }}>Outfit:</label>
        <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
          <button onClick={() => cycleOption(outfit, outfitOptions, onOutfitChange, 'prev')} style={{ padding: '5px 12px', cursor: 'pointer' }}>◀</button>
          <span style={{ minWidth: '100px', textAlign: 'center' }}>{outfit}</span>
          <button onClick={() => cycleOption(outfit, outfitOptions, onOutfitChange, 'next')} style={{ padding: '5px 12px', cursor: 'pointer' }}>▶</button>
        </div>
      </div>

      {/* Pose selector */}
      <div style={{ marginBottom: '15px', display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
        <label style={{ fontWeight: 'bold', minWidth: '80px' }}>Pose:</label>
        <select
          value={poseId || ''}
          onChange={(e) => onPoseChange(e.target.value ? Number(e.target.value) : null)}
          style={{ flex: 1, marginLeft: '10px', padding: '8px', fontSize: '14px', cursor: 'pointer' }}
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
