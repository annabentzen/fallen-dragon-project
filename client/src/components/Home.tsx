import { useNavigate } from 'react-router-dom';

export default function Home() {
  const navigate = useNavigate();

  const startStory = () => {
    const sessionId = Math.floor(Math.random() * 10000); // simple session ID for demo
    navigate(`/story/${sessionId}`);
  };

  return (
    <div className="home">
      <h1>Fallen Dragon</h1>
      <button onClick={startStory}>Start Story</button>
    </div>
  );
}
