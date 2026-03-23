# What is Geocoding.net?

Geocoding.net is a generic C# geocoding library that exposes a single interface for forward geocoding, reverse geocoding, and distance calculations across multiple providers.

## Core Design Goals

- Keep geocoding provider-agnostic through `IGeocoder` and shared model types.
- Isolate provider-specific request, response, and exception logic in each provider project.
- Preserve compatibility where possible without letting obsolete provider behavior shape the shared API.
- Stay async-native so the library fits modern ASP.NET Core, worker, and CLI applications.

## Project Layout

```text
src/
├── Geocoding.Core
├── Geocoding.Google
├── Geocoding.Here
├── Geocoding.MapQuest
├── Geocoding.Microsoft
└── Geocoding.Yahoo

test/
└── Geocoding.Tests

samples/
└── Example.Web
```

## Shared Abstractions

`Geocoding.Core` contains the interfaces and shared models that consumers code against:

- `IGeocoder` for forward and reverse geocoding.
- `IBatchGeocoder` for batch operations where supported.
- `Address`, `Location`, `Bounds`, and `Distance` for provider-agnostic data.

## Provider Packages

Each provider package owns its own extensions and service-specific details:

- `Geocoding.Google`
- `Geocoding.Microsoft`
- `Geocoding.Here`
- `Geocoding.MapQuest`
- `Geocoding.Yahoo`

When you need service-specific fields, use the provider address type rather than adding those properties to the shared models.
