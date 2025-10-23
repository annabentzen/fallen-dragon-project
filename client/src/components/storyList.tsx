import { useEffect, useState } from "react";
import { Story } from "../types/story";
import { getStories, deleteStory } from "../api/story";

export function StoryList() {
  const [stories, setStories] = useState<Story[]>([]);

  useEffect(() => {
    getStories().then(setStories);
  }, []);

  return (
    <div>
      <h1>Stories</h1>
      <ul>
        {stories.map(s => (
          <li key={s.id}>
            {s.title}{" "}
            <button onClick={() => deleteStory(s.id).then(() => setStories(stories.filter(st => st.id !== s.id)))}>
              Delete
            </button>
          </li>
        ))}
      </ul>
    </div>
  );
}
