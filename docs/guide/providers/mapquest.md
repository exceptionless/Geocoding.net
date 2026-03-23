# MapQuest

## When to Use It

Use `Geocoding.MapQuest` when you have an active MapQuest commercial integration and want the provider behind the repository's shared `IGeocoder` and `IBatchGeocoder` abstractions.

## Package

```powershell
Install-Package Geocoding.MapQuest
```

## Official References

- [MapQuest Geocoding API documentation](https://developer.mapquest.com/documentation/api/geocoding/)
- [MapQuest developer portal](https://developer.mapquest.com/)

## How to Get an API Key

1. Create or sign in to a MapQuest developer account.
2. Create an application in the MapQuest developer portal.
3. Copy the generated application key.
4. Use that key with `MapQuestGeocoder`.

## Minimal Setup

```csharp
using System.Collections.Generic;
using System.Threading;
using Geocoding;
using Geocoding.MapQuest;

CancellationToken cancellationToken = default;
IGeocoder geocoder = new MapQuestGeocoder("your-mapquest-key");
IEnumerable<Address> results = await geocoder.GeocodeAsync(
    "1600 Pennsylvania Ave NW Washington DC 20500",
    cancellationToken);
```

## Provider-Specific Features

- `MapQuestGeocoder` implements `IBatchGeocoder` for batch forward geocoding.
- The provider keeps legacy OpenStreetMap mode disabled and rejects attempts to enable it.

## Operational Notes

- Use the commercial MapQuest API only. `UseOSM = true` is intentionally rejected.
- Expect transport and status failures to include request context in the thrown exception message.
- MapQuest response ordering favors more precise results before broader regional matches.
