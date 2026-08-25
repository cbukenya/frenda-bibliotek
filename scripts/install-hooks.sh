#!/usr/bin/env bash
# scripts/install-hooks.sh
# Installs the project's git hooks from scripts/hooks/ into .git/hooks/.
# Run once after cloning the repo.

set -euo pipefail
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
HOOKS_SRC="$SCRIPT_DIR/hooks"
HOOKS_DEST="$SCRIPT_DIR/../.git/hooks"

for hook in "$HOOKS_SRC"/*; do
  name=$(basename "$hook")
  cp "$hook" "$HOOKS_DEST/$name"
  chmod +x "$HOOKS_DEST/$name"
  echo "  installed: $name"
done

echo "Git hooks installed."
