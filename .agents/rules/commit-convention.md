# Commit Message Convention

All git commit messages for this repo must follow these rules:

- **Maximum two lines** — a single sentence subject line, and one optional follow-up sentence if truly needed.
- **Story form, not a checklist** — write it as a plain English narrative describing what happened and why, not bullet points or a technical inventory.
- **No conventional commit prefixes** (no `feat:`, `chore:`, `fix:` etc.) unless explicitly requested.

## Examples

**Good:**
```
Added the data models and generated the initial EF Core migration. Schema is now versioned and ready for seeding.
```

```
Implemented the loan flow — borrow and return endpoints are working end-to-end with transaction safety.
```

**Bad:**
```
feat: add loan service

- Added BorrowBook method
- Added ReturnLoan method
- Added exceptions
```
