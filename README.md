# Shreyas Profile API

A small, public, read-only ASP.NET Core backend for Shreyas Vikrant Dewangswami's personal developer portfolio. It provides a deterministic curated quote of the day, an application health check, and Development-only OpenAPI documentation.

## Architecture

The solution uses a clean, dependency-directed structure:

`Api → Application → Domain` and `Api → Infrastructure → Application → Domain`.

The Domain project is dependency-free. Application contains the quote use case and its provider contract. Infrastructure reads the curated JSON data. API hosts Minimal API endpoints, middleware, OpenAPI, and dependency registration.

## Project structure

```text
shreyas-profile-api/
├── src/
│   ├── Shreyas.Profile.Api/
│   ├── Shreyas.Profile.Application/
│   ├── Shreyas.Profile.Domain/
│   └── Shreyas.Profile.Infrastructure/
├── tests/Shreyas.Profile.Tests/
├── Shreyas.Profile.sln
└── global.json
```

## Prerequisites

- .NET 10 SDK

## Commands

```bash
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release
dotnet run --project src/Shreyas.Profile.Api
```

## Endpoints

- `GET /health` — ASP.NET Core health check; returns HTTP 200 while healthy.
- `GET /api/quotes/today` — returns the UTC-date deterministic curated quote.

## OpenAPI

OpenAPI and Swagger UI are enabled only in the Development environment. Run the API locally, then open:

- `http://localhost:5213/openapi/v1.json` for the OpenAPI document.
- `http://localhost:5213/swagger` for Swagger UI.

Neither endpoint is mapped in Production by default.

Example response:

```json
{
  "text": "Success is the sum of small efforts repeated day in and day out.",
  "author": "Robert Collier",
  "date": "2026-09-30",
  "source": "curated"
}
```

Curated quotes are stored in `src/Shreyas.Profile.Infrastructure/Data/quotes.json`. If that data is missing or malformed, the provider logs a warning and uses a safe built-in fallback quote.

## Current scope and deferred work

This phase intentionally has no database, administration interface, authentication, or authorization because all endpoints are public and read-only. The public endpoint layout and built-in ASP.NET Core pipeline leave room for future JWT Bearer/Auth0 integration and an `Admin` authorization policy when editable content is introduced.
