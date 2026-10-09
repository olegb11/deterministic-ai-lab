# RFC 0001: Intent Compilation Pipeline for Deterministic AI-Driven Development

**Status:** Draft  
**Author:** Oleg Burlaka  
**Category:** Software Engineering Framework  
**License:** CC-BY-4.0

## Abstract

This RFC proposes the **Intent Compilation Pipeline (ICP)**, a deterministic software engineering framework for AI-assisted development.

The central premise is simple:

> Conversation is transient. Intent must be persistent.

Modern AI-assisted development often stores requirements, rationale, and design intent inside chat histories. These conversations become hidden system state and gradually introduce ambiguity, context drift, and loss of intent.

ICP proposes that human intent must be represented through executable, version-controlled artifacts. Under this model, Large Language Models (LLMs) are treated as stateless implementation engines operating on explicit constraints and verification contracts.

The framework addresses five primary sources of non-determinism:

- Intent Leakage
- Context Drift
- Specification Drift
- Test Drift
- Code Drift

## Problem Statement

Most AI-assisted workflows follow the pattern:

```text
Human Intent -> Prompt -> LLM -> Code -> Tests
```

This architecture creates several structural weaknesses:

- Intent exists primarily in chat history.
- Requirements are difficult to verify.
- Conversations become hidden state.
- Tests may validate implementation rather than intent.
- Reproducibility degrades as context evolves.

As systems grow, correctness becomes increasingly dependent on undocumented conversational context rather than version-controlled artifacts.

This phenomenon is defined as **Intent Leakage**.

## Design Goals

The Intent Compilation Pipeline aims to establish:

- Deterministic implementation behavior.
- Reproducible AI execution.
- Objective verification.
- Aggressive detection of intent leakage.
- End-to-end traceability.
- Explicit ownership boundaries between humans and AI systems.

## Core Principle

Software development is treated as a compilation process:

```text
Business Intent
    ↓
Executable Specification
    ↓
Red Tests
    ↓
Source Code
    ↓
Running System
```

Each stage is a more formal representation of the previous stage.

## Core Axioms

1. Intent is the primary artifact.
2. AI cannot own intent.
3. Only executable artifacts may carry authoritative state.
4. Passing tests prove conformance to specification.
5. Killed mutants provide evidence of intent preservation.

## Intent Compilation Pipeline Architecture

| Layer | Component | Ownership |
|---------|---------|---------|
| 0 | Domain Invariants | Human |
| 1 | Executable Specifications | Human |
| 1.5 | Technical Constraints | Human |
| 2 | Red Tests | Human |
| 3 | Green Implementation | AI Generated |

### Layer 0: Domain Invariants

Technology-independent business rules.

Examples:

- Money values cannot be negative.
- Order totals cannot be negative.
- Promo codes must be unique.

Purpose:

Prevent semantic drift before behavioral specifications are written.

### Layer 1: Executable Specifications

Business intent expressed as executable examples.

Technologies:

- Gherkin
- Cucumber
- Reqnroll

Example:

```gherkin
Scenario: Valid promo code
  Given cart total is 1000
  When customer enters promo code "SAVE10"
  Then total should be 900
```

### Layer 1.5: Technical Constraints

Non-functional requirements and architectural boundaries.

Examples:

- Performance requirements
- Security requirements
- Regulatory requirements
- Availability requirements
- Architectural constraints
- Operational constraints

Purpose:

Bridge business intent and implementation realities without polluting business specifications.

### Layer 2: Red Tests

Human-authored executable verification contracts.

Tests define the behavioral boundaries that implementations must satisfy.

### Layer 3: Green Implementation

Inputs:

- Executable Specifications
- Technical Constraints
- Types and Interfaces
- Failing Test Suite

Output:

- Passing implementation code

AI may propose implementations and design alternatives but holds no final design authority.

## Verification & Quality Gates

```text
AI Code Generation
        ↓
Gate 1: Correctness
        ↓
Gate 2: Resilience
        ↓
Definition of Done
```

### Gate 1: Correctness

Question:

Does the implementation satisfy the specification?

Criterion:

All unit, integration, and BDD tests pass.

### Gate 2: Resilience

Question:

Do tests adequately preserve intended business behavior?

Mechanism:

Mutation Testing.

Criterion:

100% of admitted non-equivalent mutants are killed.

## Scientific Interpretation

Passing tests demonstrate implementation conformance to specification.

Mutation testing increases confidence that specifications and verification contracts sufficiently preserve intended business behavior.

A surviving mutant is treated as potential Intent Leakage until proven otherwise.

## Human vs AI Authority Model

| Responsibility Domain | Human Engineer | AI Assistant |
|----------------------|----------------|-------------|
| Business Intent | Owner | Excluded |
| Domain Invariants | Owner | Excluded |
| Executable Specifications | Owner | Excluded |
| Red Tests | Owner | Excluded |
| Design Approval | Owner | Excluded |
| Implementation Alternatives | Evaluator | Proposer |
| Production Code | Reviewer | Generator |
| Refactoring Proposals | Reviewer | Proposer |

## Definition of Done

A feature is complete when:

- All BDD scenarios pass.
- All unit and integration tests pass.
- No surviving business-relevant mutants remain.
- Equivalent mutants are documented and explicitly admitted.
- E2E and system verification pass.

At this point:

```text
Intent -> Specification -> Tests -> Implementation
```

has no known semantic leakage.

## Conclusion

The Intent Compilation Pipeline reframes AI-assisted development as a deterministic compilation process.

Its central question is:

> Can human intent survive compilation into executable software without loss of fidelity?

In ICP:

- Intent is persistent.
- Conversation is transient.
- Specifications are the persistence layer for intent.
- Tests verify conformance.
- Mutation testing detects potential intent leakage.

Together these principles establish a zero-trust verification model for AI-generated software.
