# Google Maps

## When to Use It

Use `Geocoding.Google` when Google Maps Platform is already part of your operational stack or when you need Google-specific address metadata such as `GoogleAddressType`, `GoogleLocationType`, and address components.

## Package

```powershell
Install-Package Geocoding.Google
```

## Official References

- [Google Maps Geocoding API overview](https://developers.google.com/maps/documentation/geocoding/overview)
- [Create and restrict an API key](https://developers.google.com/maps/documentation/geocoding/get-api-key)
- [Google Maps Platform console](https://console.cloud.google.com/google/maps-apis)

## How to Get an API Key

1. Create or select a Google Cloud project.
2. Enable the Geocoding API for that project.
3. Create an API key in the Google Cloud console.
4. Apply application and API restrictions before using the key in production.

## Minimal Setup

```csharp
using System.Collections.Generic;
using System.Threading;
using Geocoding;
using Geocoding.Google;

CancellationToken cancellationToken = default;
IGeocoder geocoder = new GoogleGeocoder("your-google-api-key");
IEnumerable<Address> results = await geocoder.GeocodeAsync(
    "1600 Pennsylvania Ave NW Washington DC 20500",
    cancellationToken);
```

## Provider-Specific Features

- `GoogleAddress` exposes address components and partial-match signals.
- `GoogleComponentFilter` supports country, postal-code, and administrative-area filtering.
- `BusinessKey` remains available for signed Google Maps client compatibility.

## Operational Notes

- New integrations should prefer API keys over signed client credentials.
- Restrict the key at the Google Cloud layer; Geocoding.net does not replace vendor-side credential hygiene.
- Expect quota, billing, and request-denied failures to surface as `GoogleGeocodingException`.
