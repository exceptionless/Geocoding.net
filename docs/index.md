---
layout: home

hero:
  name: Geocoding.net
  text: Provider-agnostic geocoding for .NET
  tagline: One interface for forward geocoding, reverse geocoding, and migration-aware provider support across modern and compatibility services.
  actions:
    - theme: brand
      text: Get Started
      link: /guide/getting-started
    - theme: alt
      text: Compare Providers
      link: /guide/providers

features:
  - title: Unified API
    details: Build against shared abstractions in Geocoding.Core while swapping concrete provider implementations per environment.
  - title: Provider Playbooks
    details: Each provider guide covers package selection, account setup, credential provisioning, operational caveats, and migration notes.
  - title: Async Native
    details: Public APIs are async-first and designed for modern .NET applications, services, and background workers.
  - title: Provider Isolation
    details: Each provider keeps its own request models, exception types, and address extensions without leaking into shared abstractions.
  - title: Compatibility Aware
    details: Bing Maps and Yahoo remain documented as migration and compatibility surfaces without being presented as the default choice for new integrations.
  - title: Sample App
    details: The sample web app demonstrates how to wire providers into a minimal ASP.NET Core application.
---

## Quick Example

```csharp
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Geocoding;
using Geocoding.Google;

CancellationToken cancellationToken = default;
IGeocoder geocoder = new GoogleGeocoder("your-google-api-key");
IEnumerable<Address> addresses = await geocoder.GeocodeAsync(
    "1600 Pennsylvania Ave NW Washington DC 20500",
    cancellationToken);

Address first = addresses.First();
Console.WriteLine(first.FormattedAddress);
Console.WriteLine($"{first.Coordinates.Latitude}, {first.Coordinates.Longitude}");
```

## Provider Snapshot

| Provider | Status | Best For | Guide |
| --- | --- | --- | --- |
| Google Maps | Supported | Broad global coverage and provider-specific address metadata | [Google Maps](./guide/providers/google) |
| Azure Maps | Supported | New Microsoft-backed integrations | [Azure Maps](./guide/providers/azure-maps) |
| HERE | Supported | HERE Geocoding and Search API consumers | [HERE provider guide](./guide/providers/here) |
| MapQuest | Supported | Commercial MapQuest integrations | [MapQuest](./guide/providers/mapquest) |
| Bing Maps | Compatibility only | Existing enterprise deployments migrating off Bing | [Bing Maps Compatibility](./guide/providers/bing-maps) |
| Yahoo | Compatibility only | Legacy code paths you still need to retire safely | [Yahoo Compatibility](./guide/providers/yahoo) |

## Integration Checklist

1. Start with [Getting Started](./guide/getting-started) to choose packages and verify the basic calling pattern.
2. Use [Provider Support](./guide/providers) to pick the provider that matches your operational constraints.
3. Follow the provider-specific setup guide to create credentials and wire the right geocoder implementation.
4. Run the [Sample App](./guide/sample-app) when you want a minimal end-to-end verification harness.

## Learn More

- Start with [Getting Started](./guide/getting-started)
- Review the [Provider Support](./guide/providers)
- Read the provider-specific setup guides before provisioning credentials
- Run the [Sample App](./guide/sample-app) to exercise configured providers locally
