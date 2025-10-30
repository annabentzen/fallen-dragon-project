import { BrowserRouter, Routes, Route } from 'react-router-dom';
import StoryPage from './components/StoryPage';
import EndingScreen from './components/EndingScreen';
import Home from './components/Home';



// Main App component that defines which page shows for each URL
function App() {
  return (
    <BrowserRouter>
      <Routes>
        {/* "/" = Start page */}
        <Route path="/" element={<Home />} />

        {/* "/story/:sessionId" = Main story page 
            ":sessionId" is a URL variable used to track this player's session */}
        <Route path="/story/:sessionId" element={<StoryPage />} />

        {/* "/ending" = Ending page shown when the story is complete */}
        <Route path="/ending" element={<EndingScreen />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;

