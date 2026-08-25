# Frenda Bibliotek

A full-stack book lending application built from the borrower's perspective. Browse the library catalogue, borrow and return books, track your loans, and discover what to read next.

## Features

- **Browse Books** — Grid view of all books with real-time availability, genre filters, and author filters
- **Book Detail** — Full metadata, availability status, estimated reading time, and "others also borrowed" recommendations
- **Borrow** — Loan an available copy with a planned return date
- **Return** — Return active loans with one click
- **My Loans** — View current and past loans
- **Discover** — Top-10 most borrowed books and personalised recommendations
- **Multilingual** — Full Swedish and English support with instant switching

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | .NET 7 Web API, C# |
| ORM | Entity Framework Core |
| Database | PostgreSQL 16 |
| Frontend | Next.js 14 (App Router, TypeScript) |
| Auth | JWT Bearer tokens, BCrypt password hashing |
| i18n | next-intl (EN / SV) |
| Testing | xUnit, Testcontainers, FluentAssertions |
| Infrastructure | Docker Compose |

## Architecture

```
┌─────────────┐     ┌──────────────┐     ┌──────────────┐
│   Frontend   │────▶│   Backend    │────▶│  PostgreSQL   │
│  Next.js     │     │  .NET API    │     │              │
│  port 3000   │     │  port 5000   │     │  port 5432   │
└─────────────┘     └──────────────┘     └──────────────┘
```

**Frontend** proxies `/api/*` requests to the backend via Next.js rewrites. The backend applies EF Core migrations on startup and seeds demo data if the database is empty.

### Data Model

```
Book (1) ──▶ (N) BookCopy (1) ──▶ (N) Loan (N) ◀── (1) User
  │                                                      │
  └── Author (FK)                                        └── PasswordHash
  └── Genre  (FK, hierarchical)
```

- **Book** — Title-level metadata (title, ISBN, pages, description, cover URL)
- **BookCopy** — Individual physical copies of a book
- **Loan** — Links a BookCopy to a User; `ReturnedAt = null` means active loan
- **Genre** — Hierarchical categories (parent/child)
- **Author** — Book authors with slug for URL-friendly filtering

### API Endpoints

| Method | Path | Auth | Description |
|---|---|---|---|
| `GET` | `/api/books` | — | All books with availability |
| `GET` | `/api/books/{id}` | — | Detail with recommendations |
| `GET` | `/api/books/top` | — | Top 10 most borrowed |
| `GET` | `/api/genres/tree` | — | Genre hierarchy |
| `GET` | `/api/authors` | — | All authors |
| `POST` | `/api/auth/register` | — | Create account |
| `POST` | `/api/auth/login` | — | Get JWT token |
| `GET` | `/api/loans` | JWT | User's loans |
| `POST` | `/api/loans` | JWT | Borrow a book |
| `PATCH` | `/api/loans/{id}/return` | JWT | Return a loan |

## Getting Started

### Prerequisites

- [Docker](https://docs.docker.com/get-docker/) and Docker Compose

### Run

```bash
git clone https://github.com/cbukenya/frenda-bibliotek.git
cd frenda-bibliotek
bash build.sh
```

`build.sh` will:
1. Build the backend and run all integration tests (using Testcontainers)
2. Build the frontend
3. Start all services via Docker Compose

Once running:

| Service | URL |
|---|---|
| App | [http://localhost:3000](http://localhost:3000) |
| API / Swagger | [http://localhost:5001/swagger](http://localhost:5001/swagger) |
| PostgreSQL | `localhost:5433` (db: `bibliotek`) |

### Demo Users

The database is pre-populated with 8 users, 10 books, and loan history. All users share the password `password123`:

| Name | Email |
|---|---|
| Alice Lindgren | `alice@bibliotek.se` |
| Bob Eriksson | `bob@bibliotek.se` |
| Clara Svensson | `clara@bibliotek.se` |

Alice has 2 active loans — a good starting point for testing the borrow/return flow.

## Testing

Integration tests use **Testcontainers** to spin up a real PostgreSQL instance per test run — no mocks, no in-memory database. Tests cover:

- Book listing, detail, and top list endpoints
- Borrow and return flows with ownership validation
- Auth (401 for protected routes, public access for browse)

Run tests independently:

```bash
docker run --rm -v $(pwd)/backend:/src -w /src \
  mcr.microsoft.com/dotnet/sdk:7.0 dotnet test
```

## Project Structure

```
├── backend/
│   └── FrendaBibliotek.Api/
│       ├── Controllers/     # API endpoints
│       ├── Data/            # DbContext, migrations, seed
│       ├── DTOs/            # Request/response types
│       ├── Models/          # EF Core entities
│       ├── Middleware/       # User context from JWT
│       └── Services/        # Business logic (borrow, return)
│   └── FrendaBibliotek.Tests/
│       └── Integration/     # Testcontainers-based tests
├── frontend/
│   ├── src/app/[locale]/    # Next.js pages (locale-aware)
│   ├── src/components/      # Reusable UI components
│   ├── src/lib/api.ts       # API client
│   ├── src/i18n/            # Locale config
│   └── messages/            # EN/SV translations
├── docker-compose.yml
├── build.sh
└── .env
```
