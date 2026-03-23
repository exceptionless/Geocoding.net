# Bing Maps Compatibility

## Status

`BingMapsGeocoder` is retained for compatibility and migration work. New Microsoft-backed integrations should use `AzureMapsGeocoder` instead.

## Package

```powershell
Install-Package Geocoding.Microsoft
```

## Official References

- [Bing Maps REST services documentation](https://learn.microsoft.com/bingmaps/rest-services/)
- [Azure Maps migration guidance](https://learn.microsoft.com/azure/azure-maps/migrate-bing-maps-overview)

## When to Keep It

- You already have a Bing Maps enterprise deployment in production.
- You need a controlled migration window before switching to Azure Maps.
- You want to preserve behavior for existing consumers while moving new traffic elsewhere.

## Minimal Setup

```csharp
using System.Collections.Generic;
using System.Threading;
using Geocoding;
using Geocoding.Microsoft;

CancellationToken cancellationToken = default;
IGeocoder geocoder = new BingMapsGeocoder("your-bing-maps-key");
IEnumerable<Address> results = await geocoder.GeocodeAsync(
    "1600 Pennsylvania Ave NW Washington DC 20500",
    cancellationToken);
```

## Migration Guidance

1. Keep the Bing provider only for workloads that still depend on enterprise credentials.
2. Start new work on Azure Maps.
3. Compare output differences in your own application layer before cutting traffic over.

## Operational Notes

- Bing Maps failures surface as `BingGeocodingException`.
- Empty or malformed Bing payloads are handled defensively in the current implementation.
- Treat this provider as a compatibility asset, not the preferred long-term path.
