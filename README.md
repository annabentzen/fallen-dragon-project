# Fallen-dragon-project
Group project for subject Webapplications.

# The Fallen Dragon

A story-based fantasy game with branching narratives, character customization, and player choice tracking.

## Features

- JWT authentication (register/login)
- Character customization (heads, bodies, poses)
- Branching story with multiple endings
- Choice history tracking

## Tech Stack

- **Frontend:** React, TypeScript, CSS Modules
- **Backend:** ASP.NET 8, Entity Framework Core
- **Database:** SQLite

### Prerequisites

- Node.js 18+
- .NET 8 SDK

### Run the Backend
cd server
dotnet run

Server runs at http://localhost:5151

### Run the Frontend
cd client
npm run dev

## Project Structure

fallen-dragon-project-1/
├── client/                # React frontend
│   ├── public/            
│   └── src/
│       ├── components/    
│       ├── contexts/     
│       ├── services/     
│       ├── styles/       
│       └── types/         
├── server/                # ASP.NET backend
│   ├── Controllers/      
│   ├── Services/         
│   ├── Repositories/      
│   ├── Models/            
│   ├── Dtos/              
│   ├── Data/              
│   ├── Middleware/        
│   └── Seeders/           
└── docs/                  # Project documentation


