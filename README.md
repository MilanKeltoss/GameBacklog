# GameBacklog

![GameBacklog](screenshots/games-list-dark.png)

A multi-user game library manager built with ASP.NET Core MVC. Track games you want to play, are currently playing, have completed, or dropped — with automatic metadata and cover images from the RAWG video games database.

## Features

- **User authentication** via ASP.NET Core Identity — each user has their own private game collection
- **CRUD operations** for game tracking with status, platform, genre, rating, and notes
- **RAWG API integration** — search 800,000+ games and auto-fill metadata and cover images
- **Search & filter** — case-insensitive search by title and filter by status
- **Statistics dashboard** with LINQ aggregations (total games, average rating, top genre/platform)
- **Dark mode** with localStorage persistence and no flash on page load
- **Responsive design** with custom theming on top of Bootstrap 5

## Tech Stack

- **ASP.NET Core 10 MVC** — web framework
- **Entity Framework Core 10** — ORM with code-first migrations
- **ASP.NET Core Identity** — cookie-based authentication
- **SQLite** — database
- **RAWG API** — external game database integration via HttpClient
- **Bootstrap 5** — UI components with custom theming via CSS variables
- **Vanilla JavaScript** — debounced search, fetch API, theme toggle
- **Inter** typography

## Getting Started

### Prerequisites
- .NET 10 SDK
- A free RAWG API key from [rawg.io/apidocs](https://rawg.io/apidocs)

### Setup
```bash
git clone https://github.com/MilanKeltoss/GameBacklog.git
cd GameBacklog
dotnet user-secrets set "Rawg:ApiKey" "YOUR_RAWG_API_KEY"
dotnet ef database update
dotnet run
```

Then open `https://localhost:7234` in your browser, register an account, and start adding games.

## Screenshots

### Home page (light / dark)
![Home light](screenshots/home-light.png)
![Home dark](screenshots/home-dark.png)

### Game library with filtering
![Games list](screenshots/games-list-light.png)

### Statistics dashboard
![Stats](screenshots/stats-dark.png)

### Game details
![Details](screenshots/details-light.png)

### RAWG search integration
![Create](screenshots/create-light.png)

## Architecture notes

- **Layered structure** — Controllers, Services (RAWG integration), Data (DbContext), Models, ViewModels, Views
- **Dependency injection** throughout — DbContext, UserManager, IRawgService all injected
- **Async/await** for all I/O operations (database and HTTP)
- **DTO pattern** for the RAWG API response mapping, separate from domain models
- **Per-user data isolation** via UserId foreign key with IDOR-safe controller actions
- **Secrets management** via .NET user-secrets (no API keys in source control)

## Security considerations

This project applies several standard web security practices:

- **CSRF protection** — `[ValidateAntiForgeryToken]` on all POST actions, automatic token generation via Razor tag helpers
- **IDOR prevention** — every action that loads a game filters by `UserId`, never by `Id` alone, returning 404 (not 403) to avoid leaking resource existence
- **Authorization** — `[Authorize]` attribute on `GamesController` with cookie-based authentication via ASP.NET Core Identity
- **XSS-safe rendering** — RAWG search results built via DOM API (`createElement`, `textContent`) instead of `innerHTML` with template strings
- **Over-posting protection** — `[Bind]` attribute on Create/Edit actions whitelists allowed properties; server-controlled fields (`UserId`, `DateAdded`) are set on the server and excluded from model binding
- **Secrets management** — RAWG API key stored via `dotnet user-secrets` in development (never committed to source control); ready for environment variables or Azure Key Vault in production
- **Input length constraints** — `[StringLength]` attributes on user input fields to prevent DoS via oversized payloads
- **Async/await throughout** — no sync-over-async patterns that could starve the ASP.NET Core thread pool

## What I learned building this

This is my first .NET project from scratch. Key takeaways:
- How EF Core migrations work and why code-first beats ad-hoc SQL
- The role of dependency injection in keeping code testable
- How ASP.NET Core Identity integrates with custom DbContext
- Why over-posting protection (`[Bind]`) matters and how cookie auth works under the hood
- Debugging real bugs like form selector conflicts and FOUC

## Author

**Milan Keltoš** 

LinkedIn: [milan-keltoš](https://www.linkedin.com/in/milan-keltoš-7248123a9/)

---

Game data provided by [RAWG](https://rawg.io).