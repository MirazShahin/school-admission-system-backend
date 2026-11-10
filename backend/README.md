# School Admission System

A small school admission system. Guardians can pick a class, hold a seat for 10 minutes, and confirm the admission by paying the fee. Admins can create classes.

The project is split into two repositories:
- **Backend**: ASP.NET Core (.NET 8), PostgreSQL, clean architecture
- **Frontend** : React, Vite, TypeScript

## Requirements

- Docker Desktop
- Both repositories must sit side by side in the same folder:

```
D:\Assignment\
├── school-admission-system-backend\     (this repo, contains docker-compose.yml)
└── school-admission-system-frontend\    (frontend repo)
```

The compose file builds the frontend from `../school-admission-system-frontend`, so the folder name and location must match.

## Running the project

```bash
cd school-admission-system-backend
docker compose up --build
```

Once the containers are running:

| What | URL |
|---|---|
| Guardian page | http://localhost:3000 |
| Admin page | http://localhost:3000/admin |
| Backend API | http://localhost:8080 |
| Health check | http://localhost:8080/health |
| PostgreSQL (host port) | localhost:5433 |

Migrations run automatically on backend startup. No extra command is needed.

To stop the stack:

```bash
docker compose down
```

Database data is kept in the `pgdata` volume. To delete it as well, run `docker compose down -v`.

## Running tests

The backend tests start their own PostgreSQL instance with Testcontainers, so Docker must be running.

```bash
cd school-admission-system-backend
dotnet test "backend/school.slnx"
```

The tests cover:
- **Concurrency:** 50 simultaneous requests for the same seat result in exactly 10 successful holds.
- **One live application per student per class:** a second application for the same class is rejected.
- **Domain rules:** confirm, cancel, expiry, and class creation.

## Running the frontend locally (without Docker)

```bash
cd school-admission-system-frontend
npm install
npm run dev -- --port 5173 --strictPort
```

The backend's CORS policy only allows `http://localhost:3000` and `http://localhost:5173`, so use one of these ports.

## Design notes

- **Seat allocation:** When a hold is created, the class row is locked with `SELECT ... FOR UPDATE`. This prevents two requests from taking the last seat at the same time.
- **Duplicate applications:** The service checks for an existing live application first. A partial unique index (`holds_one_live_per_student_class_uq`) on `(class_id, student_id)` is a database-level backstop.
- **Expiry:** A hold's status is reported as `EXPIRED` once its `ExpiresAt` time has passed. A background service (`ExpirySweeper`) also updates expired holds in the database every 5 seconds.
- **Idempotent confirm and cancel:** Calling confirm or cancel again on the same hold does not return an error.
- **Student identity:** The student is identified by a 17-digit ID sent in the `X-Student-Id` header. There is no login.

## API

| Method | Path | Description |
|---|---|---|
| POST | `/classes` | Create a class (admin) |
| GET | `/classes` | List classes with available seats |
| GET | `/classes/{classId}` | Get one class |
| POST | `/classes/{classId}/holds` | Hold a seat for 10 minutes. Requires `X-Student-Id` |
| GET | `/holds/{holdId}` | Get one hold |
| POST | `/holds/{holdId}/confirm` | Confirm a hold (fee paid) |
| DELETE | `/holds/{holdId}` | Cancel a hold |
| GET | `/me/holds` | List the current student's applications. Requires `X-Student-Id` |

Errors use this format:

```json
{ "code": "SEAT_FULL", "message": "..." }
```

Swagger UI is only enabled in the Development environment. To run the backend locally with Swagger:

```bash
cd school-admission-system-backend
set ASPNETCORE_ENVIRONMENT=Development
dotnet run --project "backend/school.Api"
```

Then open http://localhost:8080/swagger.

## Folder structure

```
school-admission-system-backend/
├── backend/
│   ├── school.Api/              Controllers, middleware, background service, Program.cs
│   ├── school.application/      Services, DTOs, interfaces
│   ├── school.domain/           Entities, enums, domain exceptions
│   ├── school.infrastructure/   EF Core DbContext, repositories, migrations
│   ├── school.tests/            xUnit tests
│   ├── school.slnx
│   ├── Dockerfile
│   └── .dockerignore
├── docker-compose.yml
├── ASSUMPTIONS.md
└── README.md
```
