# EPSC - Pension Contribution Management System

```markdown


EPSC (Enhanced Pension System Core) is a backend solution developed as part of a technical assessment for NLPC PFA. It provides a robust and modular architecture for managing pension contributions, members, and employers while ensuring clean code practices, scalability, and maintainability.

## 🔧 Tech Stack

- **.NET Core 7.0+**
- **Entity Framework Core**
- **SQL Server**
- **Clean Architecture**
- **Domain-Driven Design (DDD)**
- **SOLID Principles**
- **Repository Pattern**
- **xUnit** for Unit Testing
- **Hangfire** for Background Jobs

---

##  Solution Structure

```

EPSC.sln
├── EPSC.API             # Entry point API (Web API)
├── EPSC.Application     # Application layer (Use cases, DTOs, Interfaces)
├── EPSC.Domain          # Domain layer (Entities, Enums, ValueObjects)
├── EPSC.Infrastructure  # Infrastructure layer (EF Core, Repositories, Migrations)
├── EPSC.Services        # Background/Utility Services (e.g. Hangfire jobs)
├── EPSC.Utility         # Shared utilities and helpers
├── EPSC.Tests           # Unit Tests

````

---

##  Core Features

### 1. Member Management
- Register new members
- Update existing member information
- Soft-delete and retrieve members
- Search and filter by relevant fields

### 2. Contribution Processing
- Capture monthly contributions
- Validate contribution periods
- Associate contributions to members/employers
- Reporting endpoints (coming soon)

### 3. Background Jobs
- Scheduled processing via **Hangfire**
- Future jobs: reconciliation, email alerts, data sync, etc.

---

##  Testing

Unit tests are written using **xUnit**. To run tests:

```bash
dotnet test
````

---

##  Getting Started

### Prerequisites

* .NET 7 SDK or higher
* SQL Server (LocalDB or full)
* Visual Studio 2022+ or VS Code

### Setup Instructions

1. Clone the repo

   ```bash
   git clone https://github.com/DevWaliyullahi/EPSC.git
   cd EPSC
   ```

2. Update connection string in:

   ```
   EPSC.API/appsettings.json
   ```

3. Run EF Core Migrations (if not yet applied):

   ```bash
   dotnet ef database update --project EPSC.Infrastructure
   ```

4. Run the app:

   ```bash
   dotnet run --project EPSC.API
   ```

---

## 📄 API Documentation

API documentation will be available via **Swagger UI** once the app is running at:

```
https://localhost:<port>/swagger
```

---

## Architectural Principles

* **Clean Architecture**: Keeps the domain and use cases at the center.
* **Separation of Concerns**: Each layer is decoupled.
* **SOLID Principles**: For maintainability and testability.
* **Repository Pattern**: Abstracts database access logic.

---





##  Author

**Lukman Waliyullahi**

---

