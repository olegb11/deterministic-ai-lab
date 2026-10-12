# 🧪 deterministic-ai-lab

**A Laboratory for Deterministic, AI-Driven TDD & BDD Methodology.**  
*Fast Loop: Specify → Red → Green → Refactor → Verify.*  
*Feature Finalization: \+ Mutation Check (**./run-tdd-cycle.sh \--full**) before the final Auto-Commit.*

## 🎯 Manifesto & Core Principles

Modern "Vibe Coding" and stateful chat-based AI development inevitably lead to **Loss of Intent**, **Context Drift**, and unmaintainable codebases.

deterministic-ai-lab validates a strict software engineering methodology for deterministic AI code generation. Under this methodology, the **LLM is not a partner in conversation, but a stateless code compilation unit**, and code validation is treated as a precise, repeatable, and objective measurement of software correctness.

### Core Principle (Intent Compilation Flow)

Software development is treated as a compilation process: \$\$\\text{Business Intent} \\longrightarrow \\text{Domain Semantics} \\longrightarrow \\text{Executable Specification} \\longrightarrow \\text{Verification Contracts} \\longrightarrow \\text{Implementation} \\longrightarrow \\text{Running System}\$\$

### Key Rules

1. **Stateless AI Execution:** Zero persistent memory in chat sessions. State is stored solely in the Git repository (Code, Tests, Specs).  
2. **Human Owns the Red Phase:** LLMs are strictly forbidden from writing unit tests or business constraints on their own from raw natural language.  
3. **Executable Specs First:** Business invariants and domain logic must be formalized in Git (`docs/specs/*.feature`) and verified via automated Reqnroll BDD acceptance tests and ontological grounding (`docs/specs/ontology.json`) before feature finalization.  
4. **Binary Compiler Arbitration:** Code is accepted ONLY if `dotnet test` (covering both BDD Reqnroll specs and unit tests) returns PASS (Green).  
5. **Human Owns the Refactor Decision:** The LLM may generate refactoring variants, but only the Human decides whether the code has genuinely improved. The automated test suite guarantees that observable behavior has not changed.  
6. **Mutation Guard (Feature Finalization Only):** A green suite is necessary but not sufficient. Test suite resilience is verified by **Stryker.NET** (`dotnet stryker`), which deliberately mutates both Domain and Application code (`src/Domain/` and `src/Application/`) and re-runs the suite for each mutant. A surviving mutant is a bug your tests missed.  
7. **Fast Loop vs. Finalization:** The ordinary TDD fast loop is never blocked by mutants. Mutation checking runs only on feature finalization (`./run-tdd-cycle.sh --full`): a surviving mutant blocks the **Auto-Commit only** \- never the code.

## 🏛️ 3-Layer Architecture & BDD Integration

Knowledge and state are passed between iterations exclusively through **formal artifacts** (C\#, Types, Executable Specs), never through ambiguous natural language prompts.

Layer 0: Domain Semantics (Ontology & Invariants) │ Layer 1: Executable Business Specs (Gherkin .feature \+ Reqnroll Integration) │ └─\> (Human Domain Translation & Step Definitions) │ Layer 2: Red Unit Tests & Rich Domain Types (xUnit) │ └─\> (Stateless LLM Payload Trigger) │ Layer 3: Minimal Green Implementation (C\# Code)

### Layer 0: Domain Semantics

Technology-independent business semantics, formal vocabulary, and rules (`ontology.json`, domain invariants, rich value objects).

### Layer 1: Executable Specifications (docs/specs/)

Human-readable yet rigorous business invariants written in Gherkin syntax, programmatically validated against domain ontology definitions (`ontology.json`) via automated architecture tests (`OntologyValidationTests`).

### Layer 2: Red Tests & Types (tests/\*.Tests/)

Human-written Red unit tests and Reqnroll step bindings translated from Layer 1\. Enforces **Rich Domain Models** and Value Objects to make invalid system states unrepresentable.

### Layer 3: Minimal Green Code (src/)

LLM generates the absolute minimum C\# implementation to pass the current failing test.

---

## 🔄 The Development Cycle (Step-by-Step)

### Fast Loop Phase Model

\$\$\\text{Specify} \\longrightarrow \\text{Red} \\longrightarrow \\text{Green} \\longrightarrow \\text{Refactor} \\longrightarrow \\text{Verify}\$\$

1. **Specify (Human):** Define or update a business rule in `docs/specs/*.feature` and semantic grounding in `ontology.json`.  
2. **Red (Human):** Add a failing test case or Reqnroll step implementation in `tests/`. Verify it fails via `dotnet test`.  
3. **Green (AI):** Implement the minimal C\# code in `src/` required to make the failing test pass.  
4. **Refactor (Human):** Human reviews code structure and design quality.  
5. **Verify (Arbiter Check):** Execute the local cross-platform transaction script (`./run-tdd-cycle.sh`).

>   
> **Invariant / Guardrail:** Any failed verification contract or arbiter check triggers an immediate, deterministic rollback to the last known green state (e.g., via `git reset --hard HEAD && git clean -fd src/`).

### Feature Finalization (`./run-tdd-cycle.sh --full`)

1. Complete the Fast Loop until the suite is green.  
2. Run `./run-tdd-cycle.sh --full`, which invokes `dotnet stryker --break-at 100`. Stryker.NET mutates `src/Domain/` and `src/Application/` per `stryker-config.json` (threshold: 100% killed mutants).  
3. **All mutants killed** → the script proceeds with the final Auto-Commit with mutation provenance.  
4. **A mutant survived** → Auto-Commit is blocked, and the Human writes an additional Red test closing the blind spot.

---

## 🧬 Mutation Agent & Test Resilience Verification

The Mutation Agent verifies test resilience by deliberately corrupting Domain and Application code, running on feature finalization only (`./run-tdd-cycle.sh --full`).

## 🛠️ Stack

- **Language:** C\# / .NET 10  
- **BDD Framework:** Reqnroll (Gherkin feature compilation via MSBuild) \+ Ontology Validation  
- **Testing Framework:** xUnit, FluentAssertions, NSubstitute  
- **Mutation Testing:** Stryker.NET (`dotnet-stryker`)  
- **AI Engine:** Stateless API Payload (Claude / OpenAI / Local LLM)  
- **Control:** POSIX Shell Script (`run-tdd-cycle.sh`)