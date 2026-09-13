# Architecture Requirements & Engineering Standards
## Project: Deterministic AI Lab Monorepo (.NET 8 & React)

> **CRITICAL INSTRUCTION FOR AI AGENT:** 
> Before generating or modifying any files, read the **Project Variable** below. You must dynamically replace the placeholder `{ProjectName}` with this actual name across **all** solution files, project folders, `.csproj` names, C# namespaces, class references, and extension methods. Do not hardcode the placeholder.

* **Project Variable:** `ProjectName` = `DeterministicAiLab`

---

### 1. Project Overview
Development of a production-grade web application template with strict separation of concerns, utilizing **Dapper** and **MS SQL** for data access via the standard `IDbConnection` abstraction. The project implements the CQRS pattern using **MediatR**, isolates DTOs and contracts into a separate project using strictly immutable types, enforces standard-based **OAuth 2.0 / OpenID Connect (OIDC)** Bearer token authentication and policy-based authorization, integrates **Swagger/OpenAPI** with OAuth2/Bearer authorization flow, includes a dedicated React + TypeScript frontend, and strictly forbids flat/monolithic entry points by enforcing a clean, modular structure for `Program.cs`.

---

### 2. Tech Stack & Architecture
* **Backend Platform:** .NET 8 (ASP.NET Core Web API)
* **Frontend Platform:** React + Vite + TypeScript
* **Architectural Pattern:** Clean Architecture / CQRS (Command Query Responsibility Segregation)
* **Mediator:** `MediatR`
* **Data Access:** Dapper with MS SQL (`Microsoft.Data.SqlClient`)
* **Database Abstraction:** Standard `System.Data.IDbConnection` (no EF Core, no direct concrete database classes in business logic).
* **Security & Identity:** OAuth 2.0 & OpenID Connect (OIDC) via `Microsoft.AspNetCore.Authentication.JwtBearer` for token validation and policy-based authorization based on claims/scopes.
* **API Documentation:** Swagger / OpenAPI with JWT Bearer / OAuth2 Security Definitions
* **Testing & Quality:** 
  * Backend: xUnit, NSubstitute (mocking), Shouldly, `WebApplicationFactory` (Integration), Stryker.NET (Mutation Testing)
  * Frontend: Vitest, React Testing Library, Playwright (E2E)

---

### 3. Solution & Monorepo Project Structure

The solution (`.sln`) must be located at the **root level** (above the `src` and `tests` directories). The projects must be structured without numeric folder prefixes:

```text
{ProjectName}.sln                               <-- Solution file at the root level
│
├── src/
│   ├── Domain/
│   │   └── {ProjectName}.Domain/               <-- Pure Domain Entities, Value Objects, Business Invariants
│   │
│   ├── Application/
│   │   ├── {ProjectName}.Contracts/            <-- Immutable DTOs (records) and API Contracts
│   │   └── {ProjectName}.Application/          <-- CQRS: Commands, Queries, Handlers (depends on IDbConnection & IUserContext)
│   │
│   ├── Infrastructure/
│   │   └── {ProjectName}.Api/                  <-- Thin controllers, modular Program.cs, Swagger, Auth, DI registrations
│   │       ├── Extensions/                     <-- Extension methods for DI, Authentication, and Middleware configuration
│   │       └── Services/                       <-- Implementation of IUserContext via HttpContextAccessor
│   │
│   └── WebUI/                                  <-- React + Vite + TypeScript Frontend App
│       ├── src/
│       │   ├── api/                            <-- Auto-generated TypeScript Contracts from Swagger
│       │   ├── components/                     <-- Presentational UI Components
│       │   └── pages/                          <-- Application Views / Route Handlers
│       └── package.json
│
└── tests/
    ├── {ProjectName}.Domain.Tests/             <-- Fast Unit Tests for Pure Domain Rules
    ├── {ProjectName}.Application.Tests/        <-- Unit tests for Handlers & CQRS (NSubstitute, DbConnection Mocks)
    ├── {ProjectName}.Api.IntegrationTests/     <-- HTTP Pipeline & Auth Integration Tests (WebApplicationFactory)
    └── {ProjectName}.WebUI.Tests/              <-- Component & E2E Tests (Vitest / Playwright)

```

---

### 4. Security & Authentication / Authorization Requirements

* **OAuth 2.0 / OpenID Connect (OIDC) Integration:** The API must strictly validate incoming OAuth 2.0 JWT Access Tokens issued by an OIDC-compliant Identity Provider (e.g., Entra ID, Keycloak, Auth0, Duende IdentityServer) using `Microsoft.AspNetCore.Authentication.JwtBearer`.
* **Configuration:** Identity Authority URL, Audience, and Scope requirements must be fully configurable via `appsettings.json`.
* **User Context Abstraction:** Business logic and Application Handlers must never directly depend on `IHttpContextAccessor` or raw ASP.NET Core classes. Instead, an `IUserContext` interface defined in `{ProjectName}.Application` must be injected to expose strongly typed user identifiers (e.g., `UserId`, `Email`, `Roles`, `Scopes`). The API layer implements this interface using `IHttpContextAccessor`.
* **Policy-Based Authorization:** Fine-grained authorization controls must be enforced via ASP.NET Core Authorization Policies configured in DI extensions and applied to controllers or endpoints via `[Authorize(Policy = "...")]` attributes.
* **Swagger Security Integration:** Swagger/OpenAPI setup in the API layer must be configured with a JWT Bearer / OAuth2 Security Scheme so developers can authorize requests directly within the Swagger UI.

---

### 5. WebUI Layer (React + TypeScript) Specific Rules

* **Physical Isolation:** React components communicate with the backend exclusively via HTTP/REST endpoints exposed by `{ProjectName}.Api`. No C# dependencies are permitted inside the `WebUI` project.
* **Contract Synchronization:** TypeScript interfaces in `src/WebUI/src/api/` MUST match contracts defined in `{ProjectName}.Contracts`. Whenever the Swagger specification changes, TypeScript client contracts must be auto-generated or synchronized.
* **UI Testing Strategy:**
* Component Unit Tests: `Vitest` + `React Testing Library` for fast feedback on component state and render logic.
* Integration / E2E Tests: `Playwright` for complete user flow validation against the API.



---

### 6. Engineering Standards & AI-Generated Workarounds Guidelines

* **Modular Program.cs Architecture Rules:**
To maintain strict separation of concerns and avoid monolithic entry points, `Program.cs` must act purely as a high-level orchestrator. All Dependency Injection (DI) registrations and middleware pipeline setup must be offloaded to standalone extension methods in `{ProjectName}.Api/Extensions/`:
* `ServiceCollectionExtensions.cs`: containing `AddInfrastructure()`, `AddApplicationServices()`, `AddAuthenticationAndAuthorization()`, and `AddSwaggerDocumentation()`.
* `WebApplicationExtensions.cs`: containing `UseCustomSwagger()`, `UseCustomAuthentication()`, `MapEndpoints()`, or custom pipeline configurations.
Inline `builder.Services.Add...` registrations or complex inline middleware logic inside `Program.cs` are strictly forbidden.


* **Mutation Testing Quality Gate:**
All Application Handlers and Domain logic must pass **Stryker.NET** mutation testing with a minimum mutation score break threshold of **100%** on feature finalization (`--break-at 100`).
* **Mandatory Documentation of Non-Standard Code / Workarounds (`// NOTE:` format):**
If an AI agent or developer implements a non-standard technical workaround, bypasses a standard framework interface in favor of an abstract base class, or handles complex framework quirks (such as Dapper's async method constraints requiring a concrete base connection rather than an interface substitute), **an explicit multi-line comment using the `NOTE` format is mandatory**. The comment structure must clearly state: (1) **The Limitation:** state clearly which framework, library, or platform constraint triggered the workaround (including exact exception messages if applicable); (2) **The Failure Mode:** explain why the standard/simpler approach fails; and (3) **The Solution Chain:** describe the alternative architecture or implementation chain applied instead.
* **Reference Implementation Standard (Dapper Mocking):**
When implementing complex or non-standard solutions, code must strictly adhere to verified structural templates. For unit-testing Dapper asynchronous data flows with mocking frameworks, the following implementation pattern is mandatory:

```csharp
// Arrange
// NOTE: Dapper's async methods (QueryAsync) do NOT work with a plain IDbConnection substitute.
// They require a connection whose CreateCommand() returns a DbCommand, otherwise Dapper throws:
//   "InvalidOperationException: Async operations require use of a DbConnection or an IDbConnection
//    where .CreateCommand() returns a DbCommand".
// Therefore we mock a DbConnection (abstract ADO.NET base class) instead of IDbConnection,
// and wire up the full DbConnection -> DbCommand -> DbDataReader chain it needs.

```