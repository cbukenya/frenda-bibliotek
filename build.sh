#!/usr/bin/env bash
set -euo pipefail

# ─── Colours ──────────────────────────────────────────────────────────────────
BOLD="\033[1m"
GREEN="\033[0;32m"
CYAN="\033[0;36m"
YELLOW="\033[0;33m"
RED="\033[0;31m"
RESET="\033[0m"

info()    { echo -e "${CYAN}${BOLD}[info]${RESET}  $*"; }
success() { echo -e "${GREEN}${BOLD}[ok]${RESET}    $*"; }
warn()    { echo -e "${YELLOW}${BOLD}[warn]${RESET}  $*"; }
error()   { echo -e "${RED}${BOLD}[error]${RESET} $*"; exit 1; }

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

# ─── Prerequisites ────────────────────────────────────────────────────────────

command -v docker   >/dev/null 2>&1 || error "Docker is not installed."
command -v dotnet   >/dev/null 2>&1 || { export PATH="$HOME/.dotnet:$PATH"; }
command -v dotnet   >/dev/null 2>&1 || error "dotnet SDK is not installed."

# ─── Env file ─────────────────────────────────────────────────────────────────

if [ ! -f .env ]; then
  warn ".env not found — copying from .env.example"
  cp .env.example .env
fi

# ─── Build & start containers ─────────────────────────────────────────────────

info "Building Docker images (no cache)..."
docker compose build --no-cache

info "Starting services (detached)..."
docker compose up -d

# ─── Wait for API to be healthy ───────────────────────────────────────────────

info "Waiting for API to become ready..."
MAX_WAIT=60
ELAPSED=0
until curl -sf http://localhost:5001/swagger/v1/swagger.json >/dev/null 2>&1; do
  if [ "$ELAPSED" -ge "$MAX_WAIT" ]; then
    error "API did not become ready within ${MAX_WAIT}s. Check: docker compose logs api"
  fi
  sleep 2
  ELAPSED=$((ELAPSED + 2))
done
success "API is ready."

# ─── Run tests ────────────────────────────────────────────────────────────────

info "Running backend tests..."
dotnet test backend/FrendaBibliotek.sln --verbosity minimal
success "All tests passed."

# ─── Print service endpoints ──────────────────────────────────────────────────

echo ""
echo -e "${BOLD}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${RESET}"
echo -e "${BOLD}  Frenda Bibliotek — Service Endpoints${RESET}"
echo -e "${BOLD}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${RESET}"
echo ""
echo -e "  ${BOLD}Frontend${RESET}       http://localhost:3000"
echo -e "  ${CYAN}The main app. Use the user switcher in the header to"
echo -e "  select a borrower, then browse books, borrow, and return.${RESET}"
echo ""
echo -e "  ${BOLD}API (Swagger)${RESET}  http://localhost:5001/swagger"
echo -e "  ${CYAN}Interactive API docs. Set X-User-Id in the Authorize"
echo -e "  dialog to identify your borrower (e.g. 1 = Alice).${RESET}"
echo ""
echo -e "  ${BOLD}API (Base URL)${RESET} http://localhost:5001/api"
echo -e "  ${CYAN}REST endpoints: /books, /books/top, /loans, /users${RESET}"
echo ""
echo -e "  ${BOLD}PostgreSQL${RESET}     localhost:5433  db=bibliotek${RESET}"
echo -e "  ${CYAN}Credentials from .env. Connect with psql or any DB client.${RESET}"
echo ""
echo -e "${BOLD}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${RESET}"
echo ""
