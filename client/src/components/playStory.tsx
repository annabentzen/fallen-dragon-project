import React from 'react';


//Purpose: Main game view for reading scenes and making choices.
//Typical usage: Displays the current scene text, shows all available ActChoice components.

const fetchAct = async (storyId: number, actId: number) => {
  const res = await fetch(`/api/story/${storyId}/acts/${actId}`);
  const data = await res.json();
  setCurrentAct(data);
};
function setCurrentAct(data: any) {
    throw new Error('Function not implemented.');
}

