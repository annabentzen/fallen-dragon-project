// src/components/StoryPage.tsx
import { useEffect, useState } from 'react';
import { getStory } from '../services/storyApi';

export default function StoryPage() {
  const [story, setStory] = useState<any>(null);

  useEffect(() => {
    getStory().then(setStory).catch(console.error);
  }, []);

  if (!story) return <div>Loading story...</div>;

  return (
    <div>
      <h1>{story.title}</h1>
      {story.acts.map((act: any) => (
        <div key={act.id}>
          <p>{act.text}</p>
          {act.choices.map((choice: any) => (
            <button key={choice.choiceId}>{choice.text}</button>
          ))}
        </div>
      ))}
    </div>
  );
}
