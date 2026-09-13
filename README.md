# 🧪 deterministic-ai-lab

> **Goal:** Eliminate hallucinations, context drift, and non-deterministic behavior in AI-generated code via binary compiler arbitration (Exit Code 0) and Stryker mutation testing.

## Key Principles
1. **Zero Trust in LLM Output** — Code is invalid until `dotnet test` and compiler say otherwise.
2. **Stateless Context** — Zero chat history reliance. Git and C# code serve as the single source of truth.
3. **Strict TDD Gatekeeping** — Red-Green-Refactor enforcement via automated runtimes.