# HERE

## When to Use It

Use `Geocoding.Here` when your platform already standardizes on HERE Geocoding and Search or when HERE-specific data contracts fit your workflow.

## Package

```powershell
Install-Package Geocoding.Here
```

## Official References

- [HERE Geocoding and Search API](https://www.here.com/docs/bundle/geocoding-and-search-api-developer-guide/page/README.html)
- [HERE account and app management](https://platform.here.com/)

## How to Get an API Key

1. Sign in to the HERE platform.
2. Create an application in your HERE project.
3. Copy the generated API key for that app.
4. Store the key in configuration rather than hard-coding it.

## Minimal Setup

```csharp
using System.Collections.Generic;
using System.Threading;
using Geocoding;
using Geocoding.Here;

CancellationToken cancellationToken = default;
IGeocoder geocoder = new HereGeocoder("your-here-api-key");
IEnumerable<Address> results = await geocoder.GeocodeAsync(
    "1600 Pennsylvania Ave NW Washington DC 20500",
    cancellationToken);
```

## Provider-Specific Features

- `HereAddress` preserves HERE-specific result details while still implementing the shared geocoding contract.
- Biasing options such as `UserLocation`, `UserMapView`, and `MaxResults` map cleanly onto HERE query parameters.

## Operational Notes

- The implementation targets the current HERE Geocoding and Search endpoints.
- Blank or missing input is rejected locally before a provider call is made.
- HERE failures surface as `HereGeocodingException`.
