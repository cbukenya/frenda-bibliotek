# EF Core Migration Strategy

## During Development (before first production deployment)

Migrations can and should be squashed freely when the schema is still evolving. Do not let migrations accumulate into a long chain of small changes during development.

**When to squash**: any time the migration list feels noisy, or before opening a PR that changes the schema.

**How to squash**:
```bash
# From backend/FrendaBibliotek.Api/
rm -rf Data/Migrations/
dotnet ef migrations add InitialCreate --output-dir Data/Migrations
```

Then commit the clean single migration. The `AppDbContextFactory` (design-time factory) ensures this always works without a running database.

## After First Production Deployment

Never delete or squash migrations once they have been applied to a production database. Always add a new migration on top:

```bash
dotnet ef migrations add <DescriptiveName> --output-dir Data/Migrations
```

## Naming Conventions

Migration names should describe what changed, not when:
- ✅ `AddUserEmailIndex`
- ✅ `AddLoanReturnedAtIndex`
- ❌ `Migration20260825`
- ❌ `Update3`

## The Design-Time Factory

`Data/AppDbContextFactory.cs` exists so that `dotnet ef` can run without a live database or app configuration. It uses hardcoded dev credentials and should never be used at runtime (it is only invoked by the CLI tooling).
