# GameBacklog

A personal game library manager built with ASP.NET Core MVC. Track games you want to play, are currently playing, have completed, or dropped.

## Features

- Add, edit, and delete games from your collection
- Track game status: Want to Play / Playing / Completed / Dropped
- Rate games on a scale of 1–10
- Search games by title (case-insensitive)
- Filter games by status
- Color-coded status badges for quick visual scanning
- User authentication via ASP.NET Core Identity (register, login, logout)
- Each user has their own private game collection

## Tech Stack

- **ASP.NET Core 10 MVC** – web framework
- **Entity Framework Core 10** – ORM with code-first migrations
- **SQLite** – database (file-based, zero-config)
- **Bootstrap 5** – UI styling
- **C# 12** – with modern features like switch expressions

## Getting Started

### Prerequisites
- .NET 10 SDK or later

### Setup
```bash
git clone https://github.com/MilanKeltoss/GameBacklog.git
cd GameBacklog
dotnet restore
dotnet ef database update
dotnet run
```

Then open `https://localhost:xxxx/Games` in your browser.

## Project Status

Work in progress. Planned features:
- Deployment to Azure App Service

## Screenshots

*(Add screenshots here)*