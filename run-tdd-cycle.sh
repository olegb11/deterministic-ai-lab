#!/usr/bin/env bash
set -e

# DeterministicAiLab - TDD Governance & Quality Gate Arbitrator (POSIX / Git Bash)
# Usage:
#   ./run-tdd-cycle.sh         -> Fast Loop (Unit Tests + Auto-Commit with Git-Trailers)
#   ./run-tdd-cycle.sh --full  -> Feature Finalization (Full Suite + Stryker + Git-Trailers)

FULL_MODE=false
if [ "$1" == "--full" ]; then
    FULL_MODE=true
fi

echo "======================================================="
echo "   === [ICP] Step 1: Running C# Test Suite ==="
echo "======================================================="
if ! dotnet test --no-restore --verbosity quiet; then
    echo "[ERROR] C# Tests failed!"
    echo "[SAFE MODE] Reverting changes in tracked files..."
    git restore src/ tests/ 2>/dev/null || true
    exit 1
fi

# Calculate SHA-256 hash of the core BDD specification for Provenance
SPEC_FILE="docs/specs/shopping-cart-policy.feature"
if [ -f "$SPEC_FILE" ]; then
    if command -v sha256sum &> /dev/null; then
        SPEC_HASH=$(sha256sum "$SPEC_FILE" | awk '{print $1}')
    elif command -v shasum &> /dev/null; then
        SPEC_HASH=$(shasum -a 256 "$SPEC_FILE" | awk '{print $1}')
    else
        SPEC_HASH="unknown"
    fi
else
    SPEC_HASH="no-spec-found"
fi

if [ "$FULL_MODE" = true ]; then
    echo ""
    echo "========================================================================="
    echo "   === [FEATURE FINALIZATION] Step 2: Stryker Mutation Testing ==="
    echo "========================================================================="
    if ! dotnet stryker --break-at 100; then
        echo "[ERROR] Mutation testing failed or score is below 100%!"
        echo "Auto-commit blocked. Fix your tests to kill all mutants."
        exit 1
    fi
    
    COMMIT_MSG="feat(tdd): feature finalization completed [100% MUTANTS KILLED]"
    MUTATION_TRAILER="Mutation-Score: 100.0%"
else
    COMMIT_MSG="chore(tdd): fast-loop auto-commit [GREEN]"
    MUTATION_TRAILER="Mutation-Score: N/A (Fast Loop)"
fi

echo ""
echo "[PASS] Quality Gates Passed. Creating Auto-Commit with Provenance..."
git add .

# Commit with standardized Git-Trailers for traceability
git commit -m "$COMMIT_MSG" \
           -m "Intent-Spec-Hash: sha256:$SPEC_HASH" \
           -m "$MUTATION_TRAILER" \
           -m "Pipeline-Verdict: DETERMINISTIC_COMPILATION_PASSED"

exit 0