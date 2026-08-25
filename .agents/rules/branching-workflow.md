# Branching & PR Workflow

All code changes go through a pull request. No direct commits to `main`.

## Branch Naming

| Type | Pattern | Example |
|---|---|---|
| Feature | `feature/<short-description>` | `feature/loan-flow` |
| Bug fix | `bug/<short-description>` | `bug/double-return-409` |
| Chore | `chore/<short-description>` | `chore/rename-borrower-to-user` |

## Workflow

1. Cut a branch from `main` using the naming convention above.
2. Make changes on the branch.
3. Open a PR to `main` when the work is ready.
4. Merge to `main` only via the PR — never push directly.

## Commit Messages on Branches

Follow the same story-form convention defined in `commit-convention.md`. Max two lines, plain English narrative.
