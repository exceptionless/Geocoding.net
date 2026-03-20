# Agent Guidelines for Geocoding.net

You are an expert .NET engineer working on Geocoding.net, a generic C# geocoding API library used by many developers. Your changes must maintain backward compatibility, reliability, and clean abstractions across multiple geocoding providers. Approach each task methodically: research existing patterns, make surgical changes, and validate thoroughly.

**Craftsmanship Mindset**: Every line of code should be intentional, readable, and maintainable. Write code you'd be proud to have reviewed by senior engineers. Prefer simplicity over cleverness. When in doubt, favor explicitness and clarity.

## Repository Overview

Geocoding.net provides a unified interface for geocoding and reverse geocoding across multiple providers:

- **Core** (`Geocoding.Core`) - `IGeocoder` interface, `Address`, `Location`, distance calculations
- **Google Maps** (`Geocoding.Google`) - Google Maps Geocoding API
- **Bing Maps** (`Geocoding.Microsoft`) - Bing Maps / Virtual Earth API
- **HERE** (`Geocoding.Here`) - HERE Geocoding API
- **MapQuest** (`Geocoding.MapQuest`) - MapQuest Geocoding API (commercial & OpenStreetMap)
- **Yahoo** (`Geocoding.Yahoo`) - Yahoo BOSS Geo Services

Design principles: **interface-first**, **provider-agnostic**, **swappable implementations**, **async-native**.

## Quick Start

```bash
# Build
dotnet build

# Test
dotnet test

# Format code
dotnet format
```

## Project Structure

```text
src/
├── Geocoding.Core              # IGeocoder interface, Address, Location, Distance
├── Geocoding.Google            # Google Maps geocoding provider
├── Geocoding.Here              # HERE geocoding provider
├── Geocoding.MapQuest          # MapQuest geocoding provider
├── Geocoding.Microsoft         # Bing Maps geocoding provider
└── Geocoding.Yahoo             # Yahoo geocoding provider
test/
└── Geocoding.Tests             # xUnit tests for all providers
examples/
└── Example.Web                 # Sample web application
```

## Coding Standards

### Style & Formatting

- Follow `.editorconfig` rules and [Microsoft C# conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Run `dotnet format` to auto-format code
- Match existing file style; minimize diffs
- No code comments unless necessary—code should be self-explanatory

### Architecture Patterns

- **Interface-first design**: All geocoding goes through `IGeocoder` (and `IBatchGeocoder` for batch operations)
- **Provider isolation**: Each provider lives in its own project with its own address type (e.g., `GoogleAddress`, `BingAddress`)
- **Naming**: `Geocoding.[Provider]` for projects, provider-specific types prefixed with provider name
- **CancellationToken**: Last parameter, defaulted to `default` in public APIs

### Code Quality

- Write complete, runnable code—no placeholders, TODOs, or `// existing code...` comments
- Use modern C# features where the target frameworks support them
- Follow SOLID, DRY principles; remove unused code and parameters
- Clear, descriptive naming; prefer explicit over clever
- Handle cancellation tokens properly: pass through call chains
- Always dispose resources: use `using` statements

### Common Patterns

- **Async suffix**: All async methods end with `Async` (e.g., `GeocodeAsync`, `ReverseGeocodeAsync`)
- **Provider-specific data**: Each provider exposes its own `Address` subclass with additional properties
- **Exception types**: Each provider has its own exception type (e.g., `GoogleGeocodingException`, `BingGeocodingException`)
- **JSON parsing**: Providers use `Newtonsoft.Json` for API response parsing

## Making Changes

### Before Starting

1. **Gather context**: Read related files, search for similar implementations, understand the full scope
2. **Research patterns**: Find existing usages of the code you're modifying using grep/semantic search
3. **Understand completely**: Know the problem, side effects, and edge cases before coding
4. **Plan the approach**: Choose the simplest solution that satisfies all requirements
5. **Check dependencies**: Verify you understand how changes affect dependent code

### Pre-Implementation Analysis

Before writing any implementation code, think critically:

1. **What could go wrong?** Consider network failures, malformed API responses, rate limits, edge cases
2. **What assumptions am I making?** Validate each assumption against the codebase
3. **Is this the root cause?** Don't fix symptoms—trace to the core problem
4. **Is there existing code that does this?** Search before creating new utilities

### Test-First Development

**Always write or extend tests before implementing changes:**

1. **Find existing tests first**: Search for tests covering the code you're modifying
2. **Extend existing tests**: Add test cases to existing test classes when possible
3. **Write failing tests**: Create tests that demonstrate the bug or missing feature
4. **Implement the fix**: Write minimal code to make tests pass
5. **Refactor**: Clean up while keeping tests green

### Validation

Before marking work complete, verify:

1. **Builds successfully**: `dotnet build` exits with code 0
2. **All tests pass**: `dotnet test` shows no failures
3. **No new warnings**: Check build output for new compiler warnings
4. **API compatibility**: Public API changes are intentional and backward-compatible when possible

## Security

- **Validate all inputs**: Use guard clauses, check bounds, validate formats before processing
- **Sanitize external data**: Never trust data from external geocoding APIs
- **No sensitive data in logs**: Never log API keys, tokens, or PII
- **Use secure defaults**: Default to HTTPS for all provider API calls
- **Follow OWASP guidelines**: Review [OWASP Top 10](https://owasp.org/www-project-top-ten/)

## Testing

### Framework

- **xUnit** as the primary testing framework
- Tests cover all providers with shared base patterns (`GeocoderTest`, `AsyncGeocoderTest`)
- Provider-specific tests extend base test classes

### Running Tests

```bash
# All tests
dotnet test

# Specific test class
dotnet test --filter "FullyQualifiedName~GoogleGeocoderTest"

# With logging
dotnet test --logger "console;verbosity=detailed"
```

Note: Most geocoder tests require valid API keys configured in `test/Geocoding.Tests/settings.json`.

## Continuous Improvement

Each time you complete a task or learn important information about the project, you must update the `AGENTS.md`, `README.md`, or relevant skill files. **Only update skills if they are owned by us** (verify via `skills-lock.json` which lists third-party skills). You are **forbidden** from updating skills, configurations, or instructions maintained by third parties/external libraries.

If you encounter recurring questions or patterns during planning, document them:

- Project-specific knowledge → `AGENTS.md` or relevant skill file
- Reusable domain patterns → Create/update appropriate skill in `.agents/skills/`

## Skills

Load from `.agents/skills/<name>/SKILL.md` when working in that domain:

| Domain        | Skills                                                                              |
| ------------- | ----------------------------------------------------------------------------------- |
| .NET          | analyzing-dotnet-performance, migrate-nullable-references, msbuild-modernization    |
| Diagnostics   | dotnet-trace-collect, dump-collect, eval-performance                                |
| Testing       | run-tests                                                                           |
| Publishing    | nuget-trusted-publishing                                                            |
| Cross-cutting | security-principles, releasenotes                                                   |

## Agents

Available in `.claude/agents/`. Use `@agent-name` to invoke:

- `engineer`: Use for implementing features, fixing bugs, or making code changes — plans, TDD, implements, verify loop, ships end-to-end
- `reviewer`: Use for reviewing code quality — adversarial 4-pass analysis (security → build → correctness → style). Read-only.
- `triage`: Use for analyzing issues, investigating bugs, or answering codebase questions — impact assessment, RCA, reproduction, implementation plans
- `pr-reviewer`: Use for end-to-end PR review — zero-trust security pre-screen, dependency audit, delegates to @reviewer, delivers verdict

### Orchestration Flow

```text
engineer → TDD → implement → verify (loop until clean)
         → @reviewer (loop until 0 blockers) → commit → push → PR
         → @copilot review → CI checks → resolve feedback → merge

triage → impact assessment → deep research → RCA → reproduce
       → implementation plan → post to GitHub → @engineer

pr-reviewer → security pre-screen (before build!) → dependency audit
            → build → @reviewer (4-pass) → verdict
```

## Constraints

- Never commit secrets — use environment variables or `settings.json` (gitignored)
- Prefer additive documentation updates — don't replace strategic docs wholesale, extend them
- Maintain backward compatibility — existing consumers must not break
