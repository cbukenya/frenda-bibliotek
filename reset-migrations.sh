#!/usr/bin/env bash
# reset-migrations.sh
# Squashes all existing EF Core migrations into a single InitialCreate.
# Safe to run during development ONLY — never run this after a production deployment.

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
API_DIR="$SCRIPT_DIR/backend/FrendaBibliotek.Api"
MIGRATIONS_DIR="$API_DIR/Data/Migrations"

export DOTNET_ROOT="$HOME/.dotnet"
export PATH="$HOME/.dotnet:$HOME/.dotnet/tools:$PATH"

echo "Deleting existing migrations..."
rm -rf "$MIGRATIONS_DIR"

echo "Generating fresh InitialCreate migration..."
cd "$API_DIR"
dotnet ef migrations add InitialCreate --output-dir Data/Migrations

echo ""
echo "Done. A single clean migration is now in Data/Migrations/."
echo "Commit it with: git add backend/ && git commit"
