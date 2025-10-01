# MyApp Solution Structure

This solution follows a **multi-layer architecture** to keep things clean, testable, and scalable.

## Solution Layout

```
MyApp.sln
 ├── MyApp.Common/           → shared models/utilities
 ├── MyApp.Engine/           → core business logic
 ├── MyApp.API/              → Web API
 └── MyApp.UI/               → WinForms (UI layer)
       ├── Forms/
       ├── Controls/
       ├── Properties/
       └── Program.cs
```

---

## Responsibilities

### 1. MyApp.Common → Shared Models & Utilities
- DTOs (Data Transfer Objects)
- Enums & Constants
- Interfaces (contracts)
- Generic helpers/extensions

Rule: **No UI or API code here.**

---

### 2. MyApp.Engine → Business Logic
- Core services (e.g., NumGenEngine)
- Business rules
- Implements contracts from Common

Rule: **No UI or API code here.**

---

### 3. MyApp.API → Web API
- Controllers (HTTP endpoints)
- Dependency injection setup
- Authentication/Authorization
- Swagger (API documentation)
- Maps domain models → API responses

Rule: Keep logic in Engine, **API only forwards requests**.

---

### 4. MyApp.UI → WinForms Desktop Application
- Forms (screens)
- Controls (custom UI controls)
- Program.cs (entry point)
- Handles UI events → calls Engine or API

Rule: **No business logic here**. Only UI interaction.

---

## Responsibilities Summary

| Layer        | Handles                                    | Should NOT handle                    |
|--------------|--------------------------------------------|--------------------------------------|
| Common       | Models, DTOs, helpers, contracts           | UI, API, DB logic                     |
| Engine       | Business rules, calculations, services     | UI design, HTTP handling              |
| API          | HTTP controllers, routing, DI setup        | Heavy business logic                  |
| UI (WinForms)| User interaction, forms, controls          | Core business rules                   |

---

## Example Flow

1. **UI**: User clicks a button → calls Engine service.
2. **Engine**: Runs logic → returns result (using Common DTO).
3. **Common**: DTO passed back.
4. **UI**: Displays result to the user.

If using **API**:
- UI calls `/api/...` endpoint → API controller → Engine → Common → back to UI as JSON.

---

## Benefits
- Clear separation of concerns
- Easier to maintain & test
- UI and API can evolve independently
- Reusable Engine logic
