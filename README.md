# Game Services API

[![CI](https://github.com/Efekoo/Staj/actions/workflows/ci.yml/badge.svg)](https://github.com/Efekoo/Staj/actions/workflows/ci.yml)

A production-style backend REST API for game-like applications, built with **ASP.NET Core (.NET 9)** and **PostgreSQL**. It provides the core services a live game would need: authentication, player profiles, currency, inventory, market economy, friends, leaderboards — plus a **real-time leaderboard over SignalR**.

## Quick start

```bash
docker compose up --build
```

That's it — the API and PostgreSQL start together, migrations apply automatically, and Swagger UI is available at **http://localhost:8080/swagger**.

## Features

| Domain | What it does |
|---|---|
| **Auth** | Register / login with JWT tokens, passwords hashed with BCrypt |
| **Users** | Player profile, XP and level progression (level-ups grant coins) |
| **Currency** | Player currency balance and transactions |
| **Inventory** | Player-owned items |
| **Market** | Buying and selling items with stock and balance checks |
| **Friends** | Friend requests and friend lists |
| **Leaderboard** | Ranked top-10 standings |
| **Real-time** | SignalR hub at `/hubs/leaderboard` broadcasts `LeaderboardUpdated` standings to all connected clients whenever a player gains XP |

## Architecture

Four-layer solution:

```
Backend.sln
├── Backend.API          # Controllers, SignalR hub, middleware, Swagger, JWT setup
├── Backend.Business     # Services, AutoMapper profiles, validation
├── Backend.Core         # Entities and shared contracts
├── Backend.DataAccess   # EF Core DbContext, migrations, repositories
└── Backend.Tests        # xUnit unit + integration tests (24 tests)
```

**Tech:** ASP.NET Core Web API · Entity Framework Core · PostgreSQL · SignalR · JWT · BCrypt · AutoMapper · Docker · xUnit · GitHub Actions · Swagger / OpenAPI

## Tests

24 xUnit tests run on every push via GitHub Actions:

- **Unit tests** — XP/level-up progression math and market buy/sell rules (stock, balance, inventory) against an in-memory EF Core provider
- **Integration tests** — full HTTP flows (register → login → JWT → protected endpoints, market purchases, leaderboard) against a real PostgreSQL instance, plus a SignalR client test asserting live leaderboard broadcasts

```bash
# requires PostgreSQL on localhost:5432 (docker compose up db)
dotnet test Backend/Backend.sln
```

## Typical flow

1. `POST /api/Auth/register` — create an account (`{ "username", "email", "password" }`)
2. `POST /api/Auth/login` — receive a JWT
3. Click **Authorize** in Swagger and paste the token
4. Call any protected endpoint (inventory, market, friends, leaderboard…)
5. Connect to `/hubs/leaderboard` with a SignalR client and watch standings update live as players gain XP

## Running without Docker

Requirements: .NET 9 SDK and a local PostgreSQL (`docker compose up db` works too).

```bash
cd Backend/Backend.API
dotnet run
```

> Connection string and JWT settings in `appsettings.json` are local development values. Override them with environment variables (e.g. `ConnectionStrings__DefaultConnection`) for any real deployment.
