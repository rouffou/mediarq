# Versioning & public API policy

Mediarq is a set of NuGet libraries. Every package follows [Semantic Versioning](https://semver.org/):
`MAJOR.MINOR.PATCH`.

- **MAJOR** — a breaking change: a public API signature or behavior changes in a way that can break a
  consumer at compile time or at runtime (renamed/removed public type or member, changed pipeline
  ordering, a behavior that now runs where it didn't before, ...).
- **MINOR** — a backward-compatible addition: a new package, a new public type/member, a new optional
  overload.
- **PATCH** — a backward-compatible bug fix: no public API change.

All packages are at `Version 1.0.0`: the public API (the `Mediarq.*` namespaces exposed by each package)
is **frozen**. See [`docs/BRANCHING.md`](../../docs/BRANCHING.md#api-publique--rappel) for the merge-time
rule — no public renaming/signature change lands on `dev` without an assumed major bump.

## Enforcement: PublicApiAnalyzers

Every package under `src/` references
[`Microsoft.CodeAnalysis.PublicApiAnalyzers`](https://github.com/dotnet/roslyn-analyzers/blob/main/src/PublicApiAnalyzers/PublicApiAnalyzers.Help.md)
(wired once for all packages via [`src/Directory.Build.props`](../../src/Directory.Build.props)). Each
package carries two files next to its `.csproj`:

- **`PublicAPI.Shipped.txt`** — the public API surface of the last **released** version. Treat this file
  like a changelog: once a version ships, its entries move here and stay.
- **`PublicAPI.Unshipped.txt`** — public API added since the last release, not yet shipped.

The analyzer flags, at build time (as a warning, visible in the IDE — it does not fail the build):

- **`RS0016`** — a public type/member was added but isn't listed in either file yet. The IDE offers a
  code fix ("Add to public API") that appends the exact signature to `PublicAPI.Unshipped.txt`.
- **`RS0017`** — a symbol is listed in one of the files but no longer exists in the code (removed or
  renamed public API — a signal to think about whether that's a MAJOR change).
- **`RS0037`** — a `PublicAPI.*.txt` file is missing the `#nullable enable` marker (nullability
  annotations wouldn't be tracked).

## Workflow

1. **Add public API** (new type, new member, new optional parameter): build the project. The analyzer
   flags `RS0016`; use the IDE lightbulb ("Add all items to the public API files") or run
   ```bash
   dotnet format analyzers src/<Package>/<Package>.csproj --diagnostics RS0016 --severity info
   ```
   from the repo root. The entries land in that package's `PublicAPI.Unshipped.txt`.
2. **Remove or rename public API**: the analyzer flags `RS0017` for the stale entry. Removing it from
   `PublicAPI.Unshipped.txt` (if unreleased) is a non-breaking cleanup; removing it from
   `PublicAPI.Shipped.txt` means you are shipping a **breaking change** — bump `MAJOR` and call it out
   explicitly in the PR description (see `docs/BRANCHING.md`).
3. **On release** (tag pushed, package published to NuGet): fold that package's
   `PublicAPI.Unshipped.txt` entries into `PublicAPI.Shipped.txt` and reset `Unshipped.txt` to just the
   `#nullable enable` header — the same move performed once, by hand, to seed the `v1.0.0` baseline for
   every package in this repo.

## Why "via IDE" and not a CI gate

The analyzer runs as part of every `dotnet build`, so the warning is visible locally and in CI logs, but
it does not fail the build — the goal is a deliberate, visible prompt when public API changes, not a hard
gate that blocks work-in-progress branches. Reviewers still eyeball public API diffs on any PR that
touches a `src/*/PublicAPI.*.txt` file.
