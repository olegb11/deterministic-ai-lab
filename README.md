# 🧪 deterministic-ai-lab

> **Goal:** Eliminate hallucinations, context drift, and non-deterministic behavior in AI-generated code via binary compiler arbitration (Exit Code 0), BDD specifications, and Stryker mutation testing.  
>   
> 🚀 **Status: Active Research & Implementation** The core backend architecture (Domain, Application, Infrastructure, API, BDD, and Integration tests) is fully implemented and operational under .NET 10\.

---

## 🎯 Key Principles

1. **Zero Trust in LLM Output** — Code is invalid until `dotnet test` and the compiler say otherwise.  
2. **Stateless Context** — Zero chat history reliance. Git, C\# code, and Gherkin specs serve as the single source of truth.  
3. **Strict TDD Gatekeeping** — Red-Green-Refactor enforcement via automated POSIX runtimes.

---

## 🛠️ Quick Start & Testing

To run the test suite locally across platforms (Linux, macOS, Windows via Git Bash / WSL):

\# Run standard test pipeline (Restore, Build, Test)

./run-tests.sh

\# Run TDD fast loop with automated Git Provenance trailers

./run-tdd-cycle.sh

\# Run Feature Finalization including Stryker.NET mutation testing (100% threshold)

./run-tdd-cycle.sh \--full

---

## 📚 Documentation

For in-depth details on the project's design and operational principles, refer to:

* [**ARCHITECTURE.md**](http://docs/ARCHITECTURE.md) — Architectural overview, core components, and system design patterns.  
* [**METHODOLOGY.md**](http://docs/METHODOLOGY.md) — Detailed methodologies, execution flow, TDD gatekeeping mechanisms, and mutation testing guidelines.  
* [**RFC-0001 (Intent Compilation Pipeline)**](http://docs/rfcs/RFC-0001-ICP-Public-Draft.md) — Core engineering framework specification.