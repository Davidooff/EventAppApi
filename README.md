# EventAppApi

REST API for an event platform: events with schedules, speakers, audiences and categories, plus user accounts with role-based access to an admin panel. Built with ASP.NET Core 9 in a Clean Architecture layout.

The admin frontend is [EventAdminWeb](https://github.com/Davidooff/EventAdminWeb).

## Stack

- **ASP.NET Core 9** Web API, controllers, OpenAPI
- **Entity Framework Core 9** + **PostgreSQL** (Npgsql), code-first migrations
- **ASP.NET Core Identity** for users and password hashing
- **JWT** access and refresh tokens, delivered as cookies
- **Redis Stack** (StackExchange.Redis + NRedisStack / RediSearch) for sessions and a user cache
- Docker image (multi-stage build)

## Architecture

```
Api/             Controllers, authorization handler, exception filter, DI setup (Program.cs)
Application/     Services (Auth, Event, User, Category), DTOs, interfaces, exceptions
Domain/          Entities (Event, Schedule, Speaker, Audience, Category, User), enums, options
Infrastructure/  EF Core DbContext + migrations, Redis services, token and identity services
```

Dependencies point inward: `Api → Application → Domain`, and `Infrastructure` implements the interfaces declared in `Application`.

## How authentication works

1. `POST /api/auth/login` or `/api/auth/register` creates a **session** in Redis and returns an access token and a refresh token as cookies.
2. Both tokens carry the session id. Every protected request checks that the session still exists in Redis, so deleting a session logs that device out immediately, even if its JWT is still valid.
3. `POST /api/auth/refresh` issues a new token pair for an existing session.
4. Sessions are stored as Redis hashes indexed by `UserId` (RediSearch), so all sessions of one user can be found and revoked together.

## Role-based access

Users have an access level: `User < Host < Moderator < Admin < SuperAdmin`. Endpoints declare the minimum level with a custom attribute:

```csharp
[HttpPost("schedule")]
[AdminLevelAuth(EUserPermissions.Host)]
public async Task<IActionResult> AddSchedule(Schedule schedule) { ... }
```

`AdminAuthHandler` loads the user from the Redis cache (falling back to PostgreSQL), checks the session and compares levels. `GET /api/auth/admin` returns which admin panels (Events, Users, Categories) the current user can open, and the admin UI builds its menu from that response.

## Endpoints

| Method | Route | Access |
|---|---|---|
| POST | `/api/auth/register`, `/api/auth/login`, `/api/auth/refresh` | public |
| GET | `/api/auth/admin` | signed in |
| GET | `/api/events` | public |
| PATCH / DELETE | `/api/events` | Host+ |
| POST / PATCH / DELETE | `/api/events/schedule` | Host+ |
| GET / PATCH | `/api/roles` | Moderator+ |

## Running locally

Requirements: .NET 9 SDK, PostgreSQL, and Redis Stack (the session index needs the RediSearch module):

```bash
docker run -d -p 5432:5432 -e POSTGRES_PASSWORD=postgres postgres
docker run -d -p 6379:6379 redis/redis-stack-server
```

Set the connection string and JWT secret in `Api/appsettings.json`, or override them with environment variables (`Database__ConnectionString`, `Jwt__Secret`). Then apply migrations and start the API:

```bash
dotnet ef database update --project Infrastructure --startup-project Api
dotnet run --project Api        # http://localhost:5009/api
```

## Status

A learning project from summer 2025, covering clean architecture, Identity, and session-backed JWT auth. The core auth, roles and event and schedule management work. Category and speaker management are not yet exposed through the API.
