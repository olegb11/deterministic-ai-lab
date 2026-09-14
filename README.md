# 🧪 deterministic-ai-lab

> **Goal:** Eliminate hallucinations, context drift, and non-deterministic behavior in AI-generated code via binary compiler arbitration (Exit Code 0) and Stryker mutation testing.

> ⚠️ **Status: Work in Progress (Draft)**
> This repository is currently in draft state and active development. Architecture, schema, and contracts are subject to breaking changes.

---

## 🎯 Key Principles

1. **Zero Trust in LLM Output** — Code is invalid until `dotnet test` and the compiler say otherwise.
2. **Stateless Context** — Zero chat history reliance. Git and C# code serve as the single source of truth.
3. **Strict TDD Gatekeeping** — Red-Green-Refactor enforcement via automated runtimes.

---

## 📚 Documentation

For in-depth details on the project's design and operational principles, refer to the following documents:

* **[ARCHITECTURE.md](https://github.com/olegb11/deterministic-ai-lab/blob/master/docs/ARCHITECTURE.md)** — Architectural overview, core components, and system design patterns.
* **[METHODOLOGY.md](https://github.com/olegb11/deterministic-ai-lab/blob/master/docs/METHODOLOGY.md)** — Detailed methodologies, execution flow, TDD gatekeeping mechanisms, and mutation testing guidelines.

---

