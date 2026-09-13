# 🧪 deterministic-ai-lab

> **A Laboratory for Deterministic, AI-Driven TDD Methodology.**  
> *Fast Loop: Red Test -> Green Code -> (Test Fail? -> Rollback) -> Refactor -> Auto-Commit.*  
> *Feature Finalization: + Mutation Check (`run-tdd-cycle.cmd --full`) before the final Auto-Commit.*

## 🎯 Manifesto & Core Principles

Modern "Vibe Coding" and stateful chat-based AI development inevitably lead to **Loss of Intent**, **Context Drift**, and unmaintainable codebases. 

`deterministic-ai-lab` validates a strict, deterministic software engineering methodology where **LLM is not a partner in conversation, but a stateless code compilation unit**.

### Key Rules
1. **Stateless AI Execution:** Zero persistent memory in chat sessions. State is stored solely in the Git repository (Code, Tests, Specs).
2. **Human Owns the Red Phase:** LLMs are strictly forbidden from writing unit tests or business constraints on their own from raw natural language.
3. **Executable Specs First:** Business invariants and domain logic must be formalized in Git (`docs/specs/*.feature`) before any implementation begins.
4. **Binary Compiler Arbitration:** Code is accepted ONLY if `dotnet test` (and `npm run test:unit` via Vitest for WebUI) returns `PASS` (Green). Any compiler error or failing test leads to an immediate `git rollback`. Heavy E2E tests (Playwright) run only on Feature Finalization.
5. **Human Owns the Refactor Decision:** The LLM may generate refactoring variants, but only the Human decides whether the code has genuinely improved. The automated test suite guarantees that observable behavior has not changed.
6. **Mutation Guard (Feature Finalization Only):** A green suite is necessary but not sufficient - a test can formally pass yet verify nothing (missing `Assert`, wrong condition). The Mutation Agent is powered by **Stryker.NET** (`dotnet stryker`), which deliberately mutates both `Domain` and `Application` code (`src/Domain/` and `src/Application/`) and re-runs the suite for each mutant. A surviving mutant is a bug your tests missed.
7. **Fast Loop vs. Finalization:** The ordinary TDD fast loop is never blocked by mutants. Mutation checking runs only on feature finalization (`run-tdd-cycle.cmd --full`): a surviving mutant blocks the **Auto-Commit only** - never the code (no rollback, the implementation is correct).

## 🏗️ 3-Layer Architecture

Knowledge and state are passed between iterations exclusively through **formal artifacts** (C#, Types, Executable Specs), never through ambiguous natural language prompts.

```text
Layer 1: Executable Specs & Invariants (Git / Gherkin .feature)
   │
   └─> (Human Domain Translation)
   │
Layer 2: Red Unit Tests & Rich Domain Types (xUnit / Vitest)
   │
   └─> (Stateless LLM Payload Trigger)
   │
Layer 3: Minimal Green Implementation (C# / React Code)

```

### Layer 1: Executable Specifications (`docs/specs/`)

Human-readable yet rigorous business invariants. Defines expected behaviors, boundary conditions, and domain rules.

### Layer 2: Red Tests & Types (`tests/*.Tests/`)

Human-written Red tests translated from Layer 1. Enforces **Rich Domain Models** and Value Objects to make invalid system states unrepresentable.

### Layer 3: Minimal Green Code (`src/`)

LLM generates the absolute minimum C# / TypeScript implementation to pass the current failing test.

## 📂 Monorepo Repository Structure

```text
{ProjectName}/
├── docs/
│   ├── ARCHITECTURE.md
│   ├── TDD-METHODOLOGY.md
│   └── specs/
│       ├── discount-policy.feature
│       └── checkout-flow.feature
├── src/
│   ├── Domain/
│   │   └── {ProjectName}.Domain/           # Pure Domain Entities & Invariants
│   ├── Application/
│   │   ├── {ProjectName}.Contracts/        # Immutable DTOs (records)
│   │   └── {ProjectName}.Application/      # CQRS: Commands, Queries, Handlers
│   ├── Infrastructure/
│   │   └── {ProjectName}.Api/              # Thin Controllers, Modular Program.cs
│   └── WebUI/                              # React + Vite + TypeScript Frontend
└── tests/
    ├── {ProjectName}.Domain.Tests/         # Fast Unit Tests for Pure Domain Rules
    ├── {ProjectName}.Application.Tests/    # Handler Unit Tests (NSubstitute, DbConnection Mocks)
    ├── {ProjectName}.Api.IntegrationTests/ # WebApplicationFactory Integration Tests
    └── {ProjectName}.WebUI.Tests/          # Vitest Component & Playwright E2E Tests

```

## 🔄 The Development Cycle (Step-by-Step)

### Fast Loop

Formula: Red Test -> Green Code -> (Test Fail? -> Rollback) -> Refactor -> Auto-Commit

The standard TDD cycle. It is **never blocked** by mutants; test failures trigger the standard rollback.

1. **Edit Spec (Human):** Define or update a business rule in `docs/specs/*.feature`.
2. **Write RED Test (Human):** Add a failing test case in `tests/*.Tests/`. Verify it fails via `dotnet test` or `npm run test:unit`.
3. **Generate GREEN Code (LLM):** Implement the minimal C# / TypeScript code in `src/` required to make the failing test pass.
4. **Arbiter Check:** Execute the local transaction script:
* `run-tdd-cycle.cmd`
* If tests **FAIL** -> `git reset --hard HEAD` + `git clean -fd src/` (rollback to the last green state).


5. **Refactor:** The LLM generates refactoring variants, but only the Human decides whether the code has genuinely improved. The test suite must stay green and guarantees that observable behavior has not changed.
6. **Auto-Commit:** If tests **PASS**, the script creates an automatic Git commit of the green, refactored state.

### Feature Finalization (`run-tdd-cycle.cmd --full`)

1. Complete the Fast Loop until the suite is green (code + refactor).
2. Run `run-tdd-cycle.cmd --full`, which invokes `dotnet stryker --break-at 100` and E2E Playwright tests. Stryker.NET mutates `src/Domain/` and `src/Application/` per `stryker-config.json` (threshold: 100% killed mutants).
3. **All mutants killed & E2E PASS** -> the script proceeds with the final Auto-Commit.
4. **A mutant survived** -> the script reports: *"Your tests missed a bug: [mutation description]"*:
* **No `git rollback**` - the implementation is correct, this is not a false red.
* The Auto-Commit is **blocked**.
* The **Human** writes an additional Red test closing the blind spot (Human Owns the Red Phase); then the Fast Loop resumes.



## 🧬 Mutation Agent

In ordinary TDD a test can pass (Green) yet verify nothing - for example, a forgotten `Assert` or a wrong condition. The Mutation Agent closes that gap by running **Stryker.NET** (`dotnet stryker`), which deliberately corrupts both Domain and Application code (`src/Domain/` and `src/Application/`):

* arithmetic flips: `+` <-> `-`, `>` <-> `>=`
* logical flips: `a != b` <-> `a == b`
* LINQ flips: `Sum()` -> `Max()`
* statement removal (removed method calls, removed `if` bodies)

For each mutant the agent re-runs the test suite (see `stryker-config.json`):

* **Killed:** at least one test fails -> the suite genuinely guards this behavior.
* **Survived:** all tests still pass -> the suite is blind to this behavior -> the agent reports to the Human: *"Your tests missed a bug: [mutation description]"*.

The Mutation Agent runs on feature finalization only (`run-tdd-cycle.cmd --full`), never inside the fast loop. It never rolls back the implementation - it guards the **tests**, not the code.

## 🛠️ Stack

* **Language:** C# / .NET 8+ & TypeScript (React)
* **Testing Framework:** xUnit (Assert/Shouldly), Vitest & Playwright
* **Mutation Testing:** Stryker.NET (`dotnet-stryker`)
* **AI Engine:** Stateless API Payload (Claude / OpenAI / Local LLM)
* **Control:** Windows CMD (run-tdd-cycle.cmd) / Git CLI