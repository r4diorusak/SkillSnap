# SkillSnap — Full-Stack Portfolio Application

SkillSnap is a complete portfolio management web application built with **Blazor WebAssembly** (client) and **ASP.NET Core 8** (backend API). It demonstrates modern full-stack development practices including JWT authentication, role-based authorization, Entity Framework Core with SQLite, and responsive component-based UI.

## 📋 Table of Contents
- [Project Structure](#project-structure)
- [Tech Stack](#tech-stack)
- [Quick Start](#quick-start)
- [API Endpoints](#api-endpoints)
- [Key Features](#key-features)
- [Authentication & Security](#authentication--security)
- [Database & Seeding](#database--seeding)
- [Troubleshooting](#troubleshooting)

---

## 📁 Project Structure

```
SkillSnap/
├── Client/                    # Blazor WebAssembly application
│   ├── Pages/                # Razor pages (Index.razor, Create.razor, etc.)
│   ├── Shared/               # Layout and shared components
│   ├── wwwroot/              # Static assets
│   ├── Program.cs            # Client startup configuration
│   └── SkillSnap.Client.csproj
├── Server/                   # ASP.NET Core backend API
│   ├── Controllers/          # API controllers (AuthController, PortfolioController)
│   ├── Data/                 # DbContext and initializer
│   ├── Models/               # Domain entities
│   ├── Program.cs            # Server startup, middleware configuration
│   ├── appsettings.Development.json
│   └── SkillSnap.Server.csproj
├── Shared/                   # Shared models and DTOs
│   ├── Models/               # PortfolioItem, etc.
│   └── SkillSnap.Shared.csproj
└── README.md
```

---

## 🛠️ Tech Stack

- **Frontend:** Blazor WebAssembly (.NET 8)
  - Component-based reactive UI
  - EditForm with data validation
  - ProtectedLocalStorage for secure token management

- **Backend:** ASP.NET Core 8 with:
  - JWT Bearer Token authentication
  - ASP.NET Core Identity for user/role management
  - Entity Framework Core 8 with SQLite

- **Database:** SQLite (local development)
  - Automatic migrations and seeding on startup
  - Sample users and portfolio items included

---

## 🚀 Quick Start

### Prerequisites
- .NET 8 SDK ([download](https://dotnet.microsoft.com/download/dotnet/8.0))
- PowerShell or command prompt

### 1. Clone and Restore

```powershell
cd path/to/SkillSnap
dotnet restore
```

### 2. Run the Server

```powershell
dotnet run --project Server --urls "https://localhost:5001;http://localhost:5000"
```

**Expected output:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
```

The server will:
- Create a SQLite database (`skillsnap.db`) in the project root
- Apply schema via `EnsureCreated()`
- Seed sample roles, users, and portfolio items

### 3. Run the Client (in another terminal)

```powershell
dotnet run --project Client --urls "https://localhost:7001;http://localhost:3000"
```

Then open **http://localhost:3000** in your browser.

---

## 📡 API Endpoints

### Authentication

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/auth/register` | Register new user | ❌ |
| POST | `/api/auth/login` | Login, returns JWT token | ❌ |

**Register Request:**
```json
{
  "email": "user@example.com",
  "password": "Password123!"
}
```

**Login Request:**
```json
{
  "email": "user@example.com",
  "password": "Password123!"
}
```

**Response (JWT):**
```json
{
  "access_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

### Portfolio Items

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/portfolio` | List all portfolio items | ❌ |
| GET | `/api/portfolio/{id}` | Get single item | ❌ |
| POST | `/api/portfolio` | Create new item | ✅ Bearer Token |

**Create Portfolio Item (POST /api/portfolio):**
```json
{
  "title": "Personal Website",
  "description": "A responsive portfolio site built with Blazor."
}
```

---

## ✨ Key Features

### 1. **Portfolio Management**
   - Create, read, update, delete (CRUD) portfolio items
   - Rich text descriptions
   - Timestamps for all entries
   - User ownership tracking

### 2. **Authentication & Authorization**
   - User registration with password hashing
   - JWT-based stateless authentication
   - Role-based access control (Admin, User roles)
   - Secure token management via ProtectedLocalStorage

### 3. **Responsive UI**
   - Blazor component-based architecture
   - EditForm with built-in validation
   - Navigation between pages
   - Server-side and client-side validation

### 4. **Data Persistence**
   - EF Core with SQLite (no external DB setup required)
   - Migrations support for schema evolution
   - Automatic seeding with sample data

---

## 🔐 Authentication & Security

### JWT Configuration
The server uses **HS256 (HMAC SHA-256)** JWT tokens with the following claims:
- `sub` (subject) — user ID
- `email` — user email
- `name` — username

**Token Expiry:** 6 hours (configurable in `AuthController.cs`)

### Default Sample Users
Two users are seeded automatically on first run:

| Email | Password | Role |
|-------|----------|------|
| `admin@skillsnap.local` | `Password123!` | Admin |
| `user@skillsnap.local` | `Password123!` | User |

> ⚠️ **Important:** Change the JWT key in `appsettings.Development.json` and sample credentials for production!

### Security Measures
- Password hashing via ASP.NET Core Identity
- CORS policy (currently open; restrict in production)
- HTTPS redirection enforced
- Bearer token validation on protected endpoints

---

## 💾 Database & Seeding

### Automatic Initialization
On startup, `Program.cs` calls `DbInitializer.InitializeAsync()` which:

1. **Creates tables** using `EnsureCreated()`
2. **Creates roles:** Admin, User
3. **Seeds users:** admin@skillsnap.local, user@skillsnap.local
4. **Seeds portfolio items:** Sample projects with descriptions

### Manual Database Reset
To reset the database:

```powershell
# Stop the server (Ctrl+C)
Remove-Item .\skillsnap.db -ErrorAction SilentlyContinue
Remove-Item .\skillsnap.db-wal -ErrorAction SilentlyContinue
Remove-Item .\skillsnap.db-shm -ErrorAction SilentlyContinue
# Restart the server
dotnet run --project Server
```

---

## 🧪 Testing the API

### Using PowerShell (HttpClient)

```powershell
# 1. Register a new user
$registerBody = @{
    email = "newuser@example.com"
    password = "TestPass123!"
} | ConvertTo-Json

$register = Invoke-WebRequest -Uri "http://localhost:5000/api/auth/register" `
  -Method POST -ContentType "application/json" -Body $registerBody
$register.Content

# 2. Login to get JWT token
$loginBody = @{
    email = "newuser@example.com"
    password = "TestPass123!"
} | ConvertTo-Json

$login = Invoke-WebRequest -Uri "http://localhost:5000/api/auth/login" `
  -Method POST -ContentType "application/json" -Body $loginBody
$token = ($login.Content | ConvertFrom-Json).access_token

# 3. Create a portfolio item (requires token)
$portfolioBody = @{
    title = "My First Project"
    description = "Built with Blazor and .NET"
} | ConvertTo-Json

Invoke-WebRequest -Uri "http://localhost:5000/api/portfolio" `
  -Method POST `
  -ContentType "application/json" `
  -Headers @{ "Authorization" = "Bearer $token" } `
  -Body $portfolioBody

# 4. Get all portfolio items
Invoke-WebRequest -Uri "http://localhost:5000/api/portfolio" | Select-Object -ExpandProperty Content
```

### Using Swagger UI
The server exposes Swagger API documentation at:
```
http://localhost:5000/swagger
```

---

## ⚙️ Configuration

### `appsettings.Development.json`
```json
{
  "Jwt": {
    "Key": "CHANGE_ME_REPLACE_IN_PRODUCTION",
    "Issuer": "skillsnap"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=skillsnap.db"
  }
}
```

**Production Recommendations:**
- Use Azure Key Vault or environment variables for sensitive config
- Set a strong, random JWT key (at least 32 bytes)
- Use a managed database (SQL Server, PostgreSQL, etc.)
- Enable HTTPS only, restrict CORS origins

---

## 🐛 Troubleshooting

### Issue: "SQLite Error 1: 'no such table: AspNetRoles'"
**Cause:** Database schema not created properly.

**Solution:**
```powershell
# Delete the database files
Remove-Item .\skillsnap.db* -ErrorAction SilentlyContinue
# Restart the server
dotnet run --project Server
```

### Issue: "The ASP.NET Core developer certificate is not trusted"
**Cause:** Development SSL certificate not trusted (warning only, won't block).

**Solution (optional):**
```powershell
dotnet dev-certs https --trust
```

### Issue: "Address already in use" on port 5000/5001
**Cause:** Another process is using those ports.

**Solution:**
```powershell
# Run on different ports
dotnet run --project Server --urls "https://localhost:5555;http://localhost:5555"
```

### Issue: Client can't reach Server API
**Cause:** CORS policy or URL mismatch.

**Check:**
1. Server is running on `http://localhost:5000` or `https://localhost:5001`
2. Client is calling the correct base URL
3. CORS policy is configured (check `Program.cs` in Server)

---

## 📚 Project Overview

This scaffold demonstrates:
- ✅ Full-stack authentication with JWT
- ✅ Role-based authorization
- ✅ EF Core data access with SQLite
- ✅ Blazor component architecture
- ✅ Input validation (client + server)
- ✅ Secure token storage
- ✅ API RESTful design
- ✅ Database seeding and initialization

**Next steps to enhance:**
- Add refresh token rotation and http-only cookie storage
- Implement SignalR for real-time updates
- Add more complex validation rules (FluentValidation)
- Implement pagination for large datasets
- Add unit and integration tests
- Deploy to Azure App Service + Static Web Apps

---

## 📄 License & Attribution

This project is part of the Coursera SkillSnap assignment for learning full-stack .NET development.

**Support:** For issues or questions, check the `SELF_REVIEW.md` for design decisions and challenges.
