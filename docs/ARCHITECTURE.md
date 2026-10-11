# Architecture Requirements & Engineering Standards

## Project: Deterministic AI Lab Monorepo (.NET 10 & React)

> **CRITICAL INSTRUCTION FOR AI AGENT:** Before generating or modifying any files, read the **Project Variable** below. You must dynamically replace the placeholder `{ProjectName}` with this actual name across **all** solution files, project folders, `.csproj` names, C\# namespaces, class references, and extension methods. Do not hardcode the placeholder.  
> 

* **Project Variable:** `ProjectName` \= `DeterministicAiLab`

---

### 1\. Project Overview

Development of a production-grade web application template with strict separation of concerns, utilizing **Dapper** and **PostgreSQL** for data access via the standard `IDbConnection` abstraction. The project implements the CQRS pattern using **MediatR**, isolates DTOs and contracts into a separate project using strictly immutable types, enforces standard-based **OAuth 2.0 / OpenID Connect (OIDC)** Bearer token authentication and policy-based authorization, integrates **Swagger/OpenAPI** with OAuth2/Bearer authorization flow, includes a dedicated React \+ TypeScript frontend, and strictly forbids flat/monolithic entry points by enforcing a clean, modular structure for `Program.cs`.

---

### 2\. Tech Stack & Architecture

* **Backend Platform:** .NET 10 (ASP.NET Core Web API)  
* **Frontend Platform:** React \+ Vite \+ TypeScript (Optional / Planned)  
* **Architectural Pattern:** Clean Architecture / CQRS (Command Query Responsibility Segregation)  
* **Mediator:** `MediatR`  
* **Data Access:** Dapper with PostgreSQL (`Npgsql`)  
* **Database Abstraction:** Standard `System.Data.IDbConnection` implemented via `NpgsqlConnection` (no EF Core, no direct concrete database classes in business logic).  
* **Security & Identity:** OAuth 2.0 & OpenID Connect (OIDC) via `Microsoft.AspNetCore.Authentication.JwtBearer` for token validation and policy-based authorization based on claims/scopes.  
* **API Documentation:** Swagger / OpenAPI with JWT Bearer / OAuth2 Security Definitions  
* **Testing & Quality:**  
  * BDD & Acceptance: Reqnroll (Gherkin feature files & step definitions) with automated Ontology Validation  
  * Backend Unit/Integration: xUnit, NSubstitute (mocking), FluentAssertions / Shouldly, `WebApplicationFactory` (Integration), Stryker.NET (Mutation Testing)  
  * Frontend: Vitest, React Testing Library, Playwright (E2E)

---

### 3\. Solution & Monorepo Project Structure

The solution (`.sln`) must be located at the **root level** (above the `src` and `tests` directories). The projects must be structured without numeric folder prefixes:

{ProjectName}.sln                               \<-- Solution file at the root level

│

├── docs/

│   ├── ARCHITECTURE.md

│   ├── METHODOLOGY.md

│   └── specs/                                  \<-- Executable business specifications (.feature files)

│       └── shopping-cart-policy.feature

│

├── src/

│   ├── Domain/

│   │   └── {ProjectName}.Domain/               \<-- Pure Domain Entities, Value Objects, Business Invariants

│   │

│   ├── Application/

│   │   ├── {ProjectName}.Contracts/            \<-- Immutable DTOs (records) and API Contracts

│   │   └── {ProjectName}.Application/          \<-- CQRS: Commands, Queries, Handlers (MediatR & FluentValidation)

│   │

│   ├── Infrastructure/

│   │   └── {ProjectName}.Infrastructure/       \<-- Dapper Repositories & PostgreSQL persistence

│   │

│   └── Api/

│       └── {ProjectName}.Api/                  \<-- Thin controllers, modular Program.cs, Swagger, Auth, DI

│           └── Extensions/                     \<-- Extension methods for DI, Authentication, and Pipelines

│

└── tests/

    ├── {ProjectName}.Domain.Tests/             \<-- Fast Unit Tests: Validates pure domain rules & invariants.

    ├── {ProjectName}.Application.Tests/        \<-- Unit Tests: Validates use cases, handlers, and validators (NSubstitute).

    ├── {ProjectName}.Domain.BDD.Tests/         \<-- BDD Acceptance Tests: Reqnroll step definitions & Ontology validation.

    ├── {ProjectName}.Api.IntegrationTests/     \<-- Integration Tests: WebApplicationFactory HTTP pipeline & auth flows.

    └── {ProjectName}.Infrastructure.Tests/     \<-- Infrastructure Tests: Real Dapper & PostgreSQL integration tests.

---

### 4\. Security & Authentication / Authorization Requirements

* **OAuth 2.0 / OpenID Connect (OIDC) Integration:** The API must strictly validate incoming OAuth 2.0 JWT Access Tokens issued by an OIDC-compliant Identity Provider (e.g., Entra ID, Keycloak, Auth0, Duende IdentityServer) using `Microsoft.AspNetCore.Authentication.JwtBearer`.  
* **Configuration:** Identity Authority URL, Audience, and Scope requirements must be fully configurable via `appsettings.json`.  
* **User Context Abstraction:** Business logic and Application Handlers must never directly depend on `IHttpContextAccessor` or raw ASP.NET Core classes. Instead, an `IUserContext` interface defined in `{ProjectName}.Application` must be injected to expose strongly typed user identifiers (e.g., `UserId`, `Email`, `Roles`, `Scopes`). The API layer implements this interface using `IHttpContextAccessor`.  
* **Policy-Based Authorization:** Fine-grained authorization controls must be enforced via ASP.NET Core Authorization Policies configured in DI extensions and applied to controllers or endpoints via `[Authorize(Policy = "...")]` attributes.  
* **Swagger Security Integration:** Swagger/OpenAPI setup in the API layer must be configured with a JWT Bearer / OAuth2 Security Scheme so developers can authorize requests directly within the Swagger UI.

---

### 5\. Backend & Application Layer Testing Standards

#### 5.1 Domain Unit Tests (`{ProjectName}.Domain.Tests`)

* **Objective:** Verify pure business rules, entity invariants, and Value Object behavior.  
* **Isolation:** Absolute isolation. No mocks, I/O operations, database access, or external packages are permitted.  
* **Characteristics:** Sub-millisecond execution (Zero-dependency Unit Tests).

#### 5.2 Application Layer Tests (`{ProjectName}.Application.Tests`)

The `{ProjectName}.Application.Tests` project is responsible for unit testing use cases, CQRS handlers (`IRequestHandler<TRequest, TResponse>`), validators (FluentValidation), and MediatR pipeline behaviors.

* **Isolation from External Systems:** Application layer tests must never interact with real databases, networks, file systems, or third-party APIs.  
* **Mocking External Dependencies:** All external services, infrastructure abstractions, and contracts (`IDbConnection`, `IUserContext`, external APIs) must be substituted using **NSubstitute**.  
* **Real Domain Objects:** Domain entities, Value Objects, and domain logic must not be mocked.

#### 5.3 Acceptance & BDD Tests (`{ProjectName}.Domain.BDD.Tests`)

* **Objective:** Execute automated validation of executable business specifications (`docs/specs/*.feature`) via Reqnroll and validate semantic grounding against `docs/specs/ontology.json`.

#### 5.4 API Integration & Infrastructure Tests

* **Integration Tests:** Validate the complete HTTP pipeline, routing, middleware pipeline, JSON serialization, and OAuth 2.0 / OIDC authentication/authorization policies in-memory via `WebApplicationFactory<Program>`.  
* **Infrastructure Tests:** Verify real database interactions via Dapper and PostgreSQL.

---

### 6\. Engineering Standards & AI-Generated Workarounds Guidelines

* **Modular Program.cs Architecture Rules:** All DI registrations and middleware pipeline setup must be offloaded to standalone extension methods in `{ProjectName}.Api/Extensions/` (`ServiceCollectionExtensions.cs`, `WebApplicationExtensions.cs`).  
* **Mutation Testing Quality Gate:** All Application Handlers and Domain logic must pass **Stryker.NET** mutation testing with a minimum mutation score break threshold of **100%** on feature finalization (`--break-at 100`).  
* **Mandatory Documentation of Non-Standard Code / Workarounds (`// NOTE:` format):** Explicit multi-line comments explaining framework constraints and implementation solutions are required when bypassing standard patterns.

---

### 7\. BDD Specifications and Execution Standards

* **Executable Specifications via Reqnroll:** Business rules are defined using Gherkin syntax (`.feature` files) and bound to backend execution via Reqnroll step definitions.  
* **Ontology Grounding Validation:** Automated architecture tests (`OntologyValidationTests`) ensure core domain terms and synonyms in feature files strictly correspond to the domain ontology (`ontology.json`).