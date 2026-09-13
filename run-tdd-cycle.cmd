@echo off
SETLOCAL EnableDelayedExpansion

:: -----------------------------------------------------------------------------
:: DeterministicAiLab - TDD Governance & Quality Gate Arbitrator (Safe Mode)
:: Usage:
::   run-tdd-cycle.cmd         -> Fast Loop (Unit Tests + Auto-Commit/Safe Revert)
::   run-tdd-cycle.cmd --full  -> Feature Finalization (Full Suite + Stryker + Auto-Commit)
:: -----------------------------------------------------------------------------

IF "%1"=="--full" GOTO FULL_CYCLE

:FAST_LOOP
echo.
echo =======================================================
echo   === [FAST LOOP] Step 1: Running C# Unit Tests ===
echo =======================================================
dotnet test --no-restore --verbosity quiet
IF %ERRORLEVEL% NEQ 0 (
    echo [ERROR] C# Unit Tests failed!
    GOTO SAFE_FAIL_HANDLER
)

echo.
echo =======================================================
echo   === [FAST LOOP] Step 2: Running React WebUI Tests ===
echo =======================================================
IF EXIST "src\WebUI\package.json" (
    pushd src\WebUI
    call npm run test
    set UI_EXIT=!ERRORLEVEL!
    popd
    IF !UI_EXIT! NEQ 0 (
        echo [ERROR] React WebUI Tests failed!
        GOTO SAFE_FAIL_HANDLER
    )
) ELSE (
    echo [SKIP] src\WebUI\package.json not found. Skipping UI tests.
)

echo.
echo [PASS] Fast Loop Green (C# + React).
echo Creating Fast-Loop Auto-Commit...
git add .
git commit -m "chore(tdd): fast-loop auto-commit [GREEN]"
exit /b 0


:FULL_CYCLE
echo.
echo =========================================================================
echo   === [FEATURE FINALIZATION] Step 1: Full Test Suite Execution ===
echo =========================================================================
dotnet test --no-restore
IF %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Full Test Suite failed!
    GOTO SAFE_FAIL_HANDLER
)

IF EXIST "src\WebUI\package.json" (
    pushd src\WebUI
    call npm run test
    set UI_EXIT=!ERRORLEVEL!
    popd
    IF !UI_EXIT! NEQ 0 (
        echo [ERROR] React WebUI Tests failed!
        GOTO SAFE_FAIL_HANDLER
    )
)

echo.
echo =========================================================================
echo   === [FEATURE FINALIZATION] Step 2: Stryker Mutation Testing Guard ===
echo =========================================================================
IF EXIST "tools\stryker\tools\net8.0\any\Stryker.CLI.dll" (
    dotnet exec tools\stryker\tools\net8.0\any\Stryker.CLI.dll --break-at 100
    IF %ERRORLEVEL% NEQ 0 (
        echo.
        echo [MUTANT SURVIVED] Mutation score is below 100%%!
        echo Fix your tests to kill all mutants. Auto-commit blocked.
        exit /b 1
    )
) ELSE (
    echo [ERROR] Stryker.CLI.dll not found in tools\stryker!
    exit /b 1
)

echo.
echo [SUCCESS] 100%% Mutants Killed & All Tests Passed!
echo Creating Feature Finalization Auto-Commit...
git add .
git commit -m "feat(tdd): feature finalization completed [100%% MUTANTS KILLED]"
exit /b 0


:SAFE_FAIL_HANDLER
echo.
echo -------------------------------------------------------------------------
echo [FAIL] Quality Gate failed!
echo [SAFE MODE] Untracked (newly created) files were KEPT on disk.
echo Reverting changes in tracked files only...
echo -------------------------------------------------------------------------
git restore src/ tests/ 2>nul
exit /b 1