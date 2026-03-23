---
name: security-principles
description: >
    Use this skill when handling provider API keys, external geocoding responses, request
    construction, logging safety, or other security-sensitive code in Geocoding.net. Apply when
    reviewing secrets handling, input validation, secure transport, or safety risks around
    external provider integrations and sample/test configuration.
---

# Security Principles

## Secrets Management

Provider credentials belong in local override files or environment variables and must never be committed to the repository.

- **Tracked placeholders** — `test/Geocoding.Tests/settings.json` is versioned and should contain placeholders only; do not put real keys there
- **Test credentials** — Keep provider API keys in `test/Geocoding.Tests/settings-override.json` or via `GEOCODING_` environment variables
- **Sample configuration** — Use placeholder values only in `samples/Example.Web/appsettings.json`
- **Environment variables** — Use environment variables for CI or local overrides when needed

```csharp
public sealed class ProviderOptions
{
    public string? ApiKey { get; set; }
}
```

## Validate All Inputs

- Check bounds and formats before processing
- Use `ArgumentNullException.ThrowIfNull()` and similar guards
- Validate early, fail fast
- Validate coordinates, address fragments, and batch sizes before sending requests

## Sanitize External Data

- Never trust data from geocoding providers, user input, or sample configuration
- Validate against expected schema
- Handle missing or malformed response fields without assuming provider correctness

## No Sensitive Data in Logs

- Never log passwords, tokens, API keys, or raw provider payloads
- Log identifiers and prefixes, not full values
- Use structured logging with safe placeholders

## Use Secure Defaults

- Default to HTTPS provider endpoints
- Avoid disabling certificate or transport validation
- Require explicit opt-out for any non-secure development-only behavior

## Avoid Deprecated Cryptographic Algorithms

Use modern cryptographic algorithms:

- ❌ `MD5`, `SHA1` — Cryptographically broken
- ✅ `SHA256`, `SHA512` — Current standards

## Avoid Insecure Serialization

- ❌ `BinaryFormatter` — Insecure deserialization vulnerability
- ✅ `System.Text.Json`, `Newtonsoft.Json` — Safe serialization

## Input Bounds Checking

- Enforce minimum/maximum values on pagination or batch parameters
- Limit batch sizes to prevent resource exhaustion
- Validate string lengths before request construction

## Safe Request Construction

- URL-encode user-supplied address fragments and query parameters
- Do not concatenate secrets or untrusted input into URLs without escaping
- Preserve provider-specific signing or authentication requirements without leaking secrets into logs

## OWASP Reference

Review [OWASP Top 10](https://owasp.org/www-project-top-ten/) regularly:

1. Broken Access Control
2. Cryptographic Failures
3. Injection
4. Insecure Design
5. Security Misconfiguration
6. Vulnerable and Outdated Components
7. Identification and Authentication Failures
8. Software and Data Integrity Failures
9. Security Logging and Monitoring Failures
10. Server-Side Request Forgery
