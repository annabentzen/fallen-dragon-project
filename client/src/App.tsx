import { BrowserRouter, Routes, Route } from 'react-router-dom';
import Home from './components/Home';
import StoryPage from './components/StoryPage';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/story/:sessionId" element={<StoryPage />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;

