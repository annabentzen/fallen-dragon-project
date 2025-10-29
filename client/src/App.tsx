import { useState } from "react";
import StoryView from "./components/StoryView";
import EndingScreen from "./components/EndingScreen";

function App() {
  const [gameEnded, setGameEnded] = useState(false);

  return (
    <div>
      {gameEnded ? <EndingScreen onEnd={function (): void {
        throw new Error("Function not implemented.");
      } } /> : <StoryView onEnd={() => setGameEnded(true)} />}
    </div>
  );
}

export default App;

