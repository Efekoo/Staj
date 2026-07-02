# Game Services API

A backend REST API for game-like applications, built with **ASP.NET Core (.NET 9)**. It provides the core services a live game would need: authentication, player profiles, currency, inventory, market economy, friends, and leaderboards.

## Features

| Domain | What it does |
|---|---|
| **Auth** | Register / login with JWT tokens, passwords hashed with BCrypt |
| **Users** | Player profile management |
| **Currency** | Player currency balance and transactions |
| **Inventory** | Player-owned items |
| **Market** | Buying and selling items for in-game currency |
| **Friends** | Friend requests and friend lists |
| **Leaderboard** | Ranked player standings |

## Architecture

Four-layer solution:

```
Backend.sln
├── Backend.API          # Controllers, middleware, Swagger, JWT setup
├── Backend.Business     # Services, AutoMapper profiles, validation
├── Backend.Core         # Entities and shared contracts
└── Backend.DataAccess   # EF Core DbContext and repositories
```

**Tech:** ASP.NET Core Web API · Entity Framework Core · SQL Server · JWT · BCrypt · AutoMapper · FluentValidation · Swagger / OpenAPI

## Running locally

Requirements: .NET 9 SDK and SQL Server LocalDB (comes with Visual Studio).

```bash
cd Backend/Backend.API
dotnet run
```

Then open the Swagger UI at the URL printed in the console (e.g. `https://localhost:5001/swagger`) to explore and test every endpoint.

> The connection string and JWT settings in `appsettings.json` are local development values (LocalDB, sample signing key). Replace them for any real deployment.

## Typical flow

1. `POST /api/auth/register` — create an account
2. `POST /api/auth/login` — receive a JWT
3. Click **Authorize** in Swagger and paste the token
4. Call any protected endpoint (inventory, market, friends, leaderboard…)
