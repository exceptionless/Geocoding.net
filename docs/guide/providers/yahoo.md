# Yahoo Compatibility

## Status

`Geocoding.Yahoo` remains in the repository only for legacy compatibility. It should not be the default choice for new work.

## Package

```powershell
Install-Package Geocoding.Yahoo
```

## Official References

- [Yahoo BOSS developer archive](https://developer.yahoo.com/boss/)

## When to Keep It

- You have an existing integration that still depends on Yahoo consumer credentials.
- You need a temporary compatibility bridge while retiring that dependency.

## Minimal Setup

```csharp
using System.Collections.Generic;
using System.Threading;
using Geocoding;
using Geocoding.Yahoo;

CancellationToken cancellationToken = default;
IGeocoder geocoder = new YahooGeocoder(
    "your-consumer-key",
    "your-consumer-secret");

IEnumerable<Address> results = await geocoder.GeocodeAsync(
    "1600 Pennsylvania Ave NW Washington DC 20500",
    cancellationToken);
```

## Operational Notes

- The provider is deprecated and intentionally absent from the sample app.
- Expect transport and HTTP failures to surface through `YahooGeocodingException`.
- Plan to migrate away from Yahoo instead of expanding usage.
