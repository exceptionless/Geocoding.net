---
name: geocoding-library
description: >
    Use this skill when implementing, reviewing, or triaging changes in Geocoding.net. Covers
    provider isolation, shared geocoding abstractions, provider-specific address and exception
    types, xUnit test strategy, API-key-backed test constraints, backward compatibility, and the
    sample web app's role in the repository.
---

# Geocoding.net Library Patterns

## When to Use

- Any change under `src/`, `test/`, `samples/`, `.claude/`, or repo-owned customization files
- Bug fixes that may repeat across multiple geocoding providers
- Code reviews or triage work that needs repo-specific architecture context

## Architecture Rules

- Keep shared abstractions in `src/Geocoding.Core`
- Keep provider-specific request/response logic inside that provider's project
- Do not leak provider-specific types into `Geocoding.Core`
- Prefer extending an existing provider pattern over inventing a new abstraction
- Keep public async APIs suffixed with `Async`
- Keep `CancellationToken` as the final public parameter and pass it through the call chain

## Provider Isolation

- Each provider owns its own address type, exceptions, DTOs, and request logic
- If a bug or improvement appears in one provider, compare sibling providers for the same pattern
- Shared helpers should only move into `Geocoding.Core` when they truly apply across providers

## Backward Compatibility

- Avoid breaking public interfaces, constructors, or model properties unless the task explicitly requires it
- Preserve existing provider behavior unless the task is a bug fix with a documented root cause
- Keep exception behavior intentional and provider-specific

## Testing Strategy

- Extend existing xUnit coverage before creating new test files when practical
- Prefer targeted test runs for narrow changes
- Run the full `Geocoding.Tests` project when shared abstractions, common test bases, or cross-provider behavior changes
- Remember that some provider tests require local API keys in `test/Geocoding.Tests/settings-override.json` or `GEOCODING_` environment variables; keep the tracked `settings.json` placeholders empty
- For bug fixes, add a regression test when the affected path is covered by automated tests

## Validation Commands

```bash
dotnet build Geocoding.slnx
dotnet test --project test/Geocoding.Tests/Geocoding.Tests.csproj
dotnet build samples/Example.Web/Example.Web.csproj
```

## Sample App Guidance

- `samples/Example.Web` demonstrates the library; it should not drive core design decisions
- Only build or run the sample when the task actually touches the sample or requires manual verification there

## Customization Files

- `.claude/agents` and repo-owned skills must stay Geocoding.net-specific
- Reference only skills that exist in `.agents/skills/`
- Reference only commands, paths, and tools that exist in this workspace
- Keep customization workflows aligned with AGENTS.md