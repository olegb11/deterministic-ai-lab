#!/usr/bin/env bash

# Stop execution immediately if any command fails
set -e

echo "==> [1/3] Restoring dependencies..."
dotnet restore

echo "==> [2/3] Building solution in Release mode..."
dotnet build --no-restore --configuration Release

echo "==> [3/3] Running unit & integration tests..."
dotnet test --no-build --configuration Release --verbosity normal

echo "==> All tests executed successfully!"