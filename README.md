### Requests Management System (API + Blazor WASM)

An employee requests management system for leave and mission workflows. Built on .NET 8 with a layered architecture:

- API host: `RequestsManagementSystem` (ASP.NET Core Web API + Swagger + JWT auth)
- Client: `RequestsManagementSystem.Client` (Blazor WebAssembly, hosted by the API)
- Domain/Core: `RequestsManagementSystem.Core`
- Data access: `RequestsManagementSystem.Data` (EF Core, SQL Server, migrations)
- Business logic: `RequestsManagementSystem.Logic`
- DTOs and validations: `RequestsManagementSystem.DTOs`

---

### Prerequisites

- .NET SDK 8.0+
- SQL Server (Developer/Express/localdb or remote)
- PowerShell (Windows) or any shell

Optional (for tooling):
- EF Core tools: `dotnet tool install --global dotnet-ef`

---

### Solution Structure

```
RequestsManagementSystem-API/
  RequestsManagementSystem.sln
  RequestsManagementSystem/                 # API host
  RequestsManagementSystem.Client/          # Blazor WASM client (hosted)
  RequestsManagementSystem.Core/            # Entities, enums, interfaces, extensions
  RequestsManagementSystem.Data/            # DbContext, migrations, repositories
  RequestsManagementSystem.Logic/           # Services (business logic)
  RequestsManagementSystem.DTOs/            # DTOs, validations, view models
```

---

### Quick Start

1) Restore

```bash
dotnet restore RequestsManagementSystem.sln
```

2) Configure connection string and JWT (do NOT commit secrets)

- Preferred: use user-secrets in the API project directory `RequestsManagementSystem/`:

```bash
dotnet user-secrets init --project RequestsManagementSystem
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=.;Database=RequestsDB;Trusted_Connection=True;TrustServerCertificate=True;" --project RequestsManagementSystem
dotnet user-secrets set "Jwt:Key" "<strong-random-key>" --project RequestsManagementSystem
dotnet user-secrets set "Jwt:Issuer" "RequestManagementSystemBackEnd" --project RequestsManagementSystem
dotnet user-secrets set "Jwt:Audience" "ElsewedyEmployees" --project RequestsManagementSystem
```

- Alternatively: set environment variables for deployment environments.

3) Apply database migrations to your SQL Server

```bash
dotnet ef database update \
  --project RequestsManagementSystem.Data \
  --startup-project RequestsManagementSystem
```

4) Run the API (which also serves the hosted Blazor client)

```bash
dotnet run --project RequestsManagementSystem
```

5) Open the app

- Swagger UI: `https://localhost:5001/swagger` or `http://localhost:5000/swagger`
- Hosted Blazor app: base URL of the API (e.g., `https://localhost:5001`)

---

### Configuration

The API reads settings from `appsettings.json`, environment-specific files, user-secrets, and environment variables. Key sections:

- `ConnectionStrings:DefaultConnection`: SQL Server connection string
- `Jwt:Key` (secret), `Jwt:Issuer`, `Jwt:Audience`, `Jwt:ExpiresInMinutes`, `Jwt:refreshExpiresInDays`
- `EmployeeLevels`: Seed configuration for leave entitlements per level
- `TransactionTypes`: Supported transaction types and relationships

Example (do not use real secrets in files):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=RequestsDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "<set-via-user-secrets>",
    "Issuer": "RequestManagementSystemBackEnd",
    "Audience": "ElsewedyEmployees",
    "ExpiresInMinutes": 15,
    "refreshExpiresInDays": 15
  }
}
```

Security note: never commit real connection strings, passwords, or JWT keys. Prefer user-secrets or environment variables.

---

### Database & Migrations

- Migrations live in `RequestsManagementSystem.Data/Migrations`
- Update database (uses API as startup to load configuration):

```bash
dotnet ef database update \
  --project RequestsManagementSystem.Data \
  --startup-project RequestsManagementSystem
```

- Add a new migration:

```bash
dotnet ef migrations add <MigrationName> \
  --project RequestsManagementSystem.Data \
  --startup-project RequestsManagementSystem
```

---

### Running in Development

- Launch profiles are in `RequestsManagementSystem/Properties/launchSettings.json`
- Typical dev URLs:
  - HTTP: `http://localhost:5000`
  - HTTPS: `https://localhost:5001`

Start the API:

```bash
dotnet run --project RequestsManagementSystem
```

The hosted Blazor client is served by the API. Navigate to the API base URL.

---

### Authentication

- JWT Bearer authentication
- Obtain a token via the authentication endpoint (see Swagger) using employee credentials
- Include `Authorization: Bearer <token>` in API requests

---

### Features (high level)

- Authentication & password update
- Leave requests: regular, casual, partial (half/quarter)
- Mission requests with date/time and locations
- Manager approvals, rejections, and modifications
- Leave balance validation and annual resets
- Notifications to employees, managers, and alternates

See `📋 Requests Management System – User Stories Specification for QA-*.md` and `Requirements-*.md` for detailed stories and acceptance criteria.

---

### API Exploration

- Swagger is enabled by default in development. Use it to test endpoints and view schemas.

---

### Build, Test, Publish

Build solution:

```bash
dotnet build RequestsManagementSystem.sln -c Release
```

Publish API:

```bash
dotnet publish RequestsManagementSystem -c Release -o ./publish
```

Run published output:

```bash
dotnet ./publish/RequestsManagementSystem.dll
```

---

### Troubleshooting

- Connection errors: verify `DefaultConnection` and SQL Server availability
- 401 Unauthorized: confirm JWT settings and use a valid token
- Migrations not found: ensure commands point to `RequestsManagementSystem.Data` with API as `--startup-project`
- HTTPS dev cert issues: run `dotnet dev-certs https --trust`

