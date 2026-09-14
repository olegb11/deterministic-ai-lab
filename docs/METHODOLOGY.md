# 🧪 deterministic-ai-lab

> **A Laboratory for Deterministic, AI-Driven TDD & BDD Methodology.**  
> *Fast Loop: Red Test -> Green Code -> (Test Fail? -> Rollback) -> Refactor -> Auto-Commit.*  
> *Feature Finalization: + Mutation Check (`run-tdd-cycle.cmd --full`) before the final Auto-Commit.*

## 🎯 Manifesto & Core Principles: Structural Invariant Framework (SIF)

Modern "Vibe Coding" and stateful chat-based AI development inevitably lead to **Loss of Intent**, **Context Drift**, and unmaintainable codebases. 

`deterministic-ai-lab` validates a strict, deterministic software engineering methodology known as the **Structural Invariant Framework (SIF)**. Under SIF, the **LLM is not a partner in conversation, but a stateless code compilation unit**, and code validation is treated as **Software Metrology** — a branch of engineering dedicated to precise, repeatable, and objective measurement of software correctness.

### Key Rules of SIF
1. **Stateless AI Execution:** Zero persistent memory in chat sessions. State is stored solely in the Git repository (Code, Tests, Specs).
2. **Human Owns the Red Phase:** LLMs are strictly forbidden from writing unit tests or business constraints on their own from raw natural language.
3. **Executable Specs First (Specification Metrology):** Business invariants and domain logic must be formalized in Git (`docs/specs/*.feature`) and verified via automated Reqnroll BDD acceptance tests before feature finalization.
4. **Binary Compiler Arbitration:** Code is accepted ONLY if `dotnet test` (covering both BDD Reqnroll specs and unit tests) and `npm run test:unit` (via Vitest for WebUI) return `PASS` (Green). Any compiler error or failing test leads to an immediate `git rollback`. Heavy E2E tests (Playwright) run only on Feature Finalization.
5. **Human Owns the Refactor Decision:** The LLM may generate refactoring variants, but only the Human decides whether the code has genuinely improved. The automated test suite guarantees that observable behavior has not changed.
6. **Mutation Guard & Metrology (Feature Finalization Only):** A green suite is necessary but not sufficient - a test can formally pass yet verify nothing (missing `Assert`, wrong condition). Software metrology is enforced by **Stryker.NET** (`dotnet stryker`), which deliberately mutates both `Domain` and `Application` code (`src/Domain/` and `src/Application/`) and re-runs the suite for each mutant. A surviving mutant is a bug your tests missed.
7. **Fast Loop vs. Finalization:** The ordinary TDD fast loop is never blocked by mutants. Mutation checking runs only on feature finalization (`run-tdd-cycle.cmd --full`): a surviving mutant blocks the **Auto-Commit only** - never the code (no rollback, the implementation is correct).

## 🏗️ 3-Layer Architecture & BDD Integration

Knowledge and state are passed between iterations exclusively through **formal artifacts** (C#, Types, Executable Specs), never through ambiguous natural language prompts.

```text
Layer 1: Executable Business Specs (Gherkin .feature + Reqnroll Integration)
   │
   └─> (Human Domain Translation & Step Definitions)
   │
Layer 2: Red Unit Tests & Rich Domain Types (xUnit / Vitest)
   │
   └─> (Stateless LLM Payload Trigger)
   │
Layer 3: Minimal Green Implementation (C# / React Code)

```

### Layer 1: Executable Specifications (`docs/specs/`)

Human-readable yet rigorous business invariants written in Gherkin syntax. Defines expected behaviors, boundary conditions, and domain rules, compiled and validated via **Reqnroll**.

### Layer 2: Red Tests & Types (`tests/*.Tests/`)

Human-written Red unit tests and Reqnroll step bindings translated from Layer 1. Enforces **Rich Domain Models** and Value Objects to make invalid system states unrepresentable.

### Layer 3: Minimal Green Code (`src/`)

LLM generates the absolute minimum C# / TypeScript implementation to pass the current failing test.

*(Note: Physical monorepo structure and directory patterns are documented in the single source of truth — `docs/ARCHITECTURE.md`)*

## 🔄 The Development Cycle (Step-by-Step)

### Fast Loop

Formula: Red Test -> Green Code -> (Test Fail? -> Rollback) -> Refactor -> Auto-Commit

The standard TDD/BDD cycle. It is **never blocked** by mutants; test failures trigger the standard rollback.

1. **Edit Spec (Human):** Define or update a business rule in `docs/specs/*.feature` (e.g., cart discount policies or promo code verification rules).
2. **Write RED Test & Bindings (Human):** Add a failing test case or Reqnroll step implementation in `tests/`. Verify it fails via `dotnet test`.
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

## 🧬 Mutation Agent & Software Metrology

In ordinary TDD a test can pass (Green) yet verify nothing - for example, a forgotten `Assert` or a wrong condition. The Mutation Agent closes that gap by running **Stryker.NET** (`dotnet stryker`), acting as a metrological standard to verify test resilience by deliberately corrupting both Domain and Application code (`src/Domain/` and `src/Application/`):

* arithmetic flips: `+` <-> `-`, `>` <-> `>=`
* logical flips: `a != b` <-> `a == b`
* LINQ flips: `Sum()` -> `Max()`
* statement removal (removed method calls, removed `if` bodies)

For each mutant the agent re-runs the test suite (unit tests and Reqnroll BDD suite via `stryker-config.json`):

* **Killed:** at least one test fails -> the suite genuinely guards this behavior.
* **Survived:** all tests still play out -> the suite is blind to this behavior -> the agent reports to the Human: *"Your tests missed a bug: [mutation description]"*.

The Mutation Agent runs on feature finalization only (`run-tdd-cycle.cmd --full`), never inside the fast loop. It never rolls back the implementation - it guards the **tests**, not the code.

## 🛠️ Stack

* **Language:** C# / .NET 10 & TypeScript (React)
* **BDD Framework:** Reqnroll (Gherkin feature compilation via MSBuild)
* **Testing Framework:** xUnit (Assert/Shouldly/FluentAssertions), Vitest & Playwright
* **Mutation Testing:** Stryker.NET (`dotnet-stryker`)
* **AI Engine:** Stateless API Payload (Claude / OpenAI / Local LLM)
* **Control:** Windows CMD (run-tdd-cycle.cmd) / Git CLI

```
