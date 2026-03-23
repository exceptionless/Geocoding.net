# Getting Started

## Choose Packages

Install the provider package that matches the service you want to call. Each provider package references the shared core abstractions.

```powershell
Install-Package Geocoding.Google
Install-Package Geocoding.Microsoft
Install-Package Geocoding.Here
Install-Package Geocoding.MapQuest
```

Install `Geocoding.Yahoo` only when you are maintaining a legacy compatibility flow.

## Pick a Starting Provider

- Start with Azure Maps when you want the actively supported Microsoft-backed provider.
- Start with Google Maps when you need Google's result model or you already operate on Google Cloud.
- Start with HERE or MapQuest when those services are already part of your stack.
- Treat Bing Maps and Yahoo as compatibility paths, not default choices for new work.

## Forward Geocoding

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
```

## Reverse Geocoding

```csharp
using System.Collections.Generic;
using System.Threading;
using Geocoding;
using Geocoding.Microsoft;

CancellationToken cancellationToken = default;
IGeocoder geocoder = new AzureMapsGeocoder("your-azure-maps-key");
IEnumerable<Address> addresses = await geocoder.ReverseGeocodeAsync(
    38.8976777,
    -77.036517,
    cancellationToken);
```

## Provider-Specific Data

Provider packages expose address types with service-specific fields. For example, Google results can be queried using `GoogleAddressType`:

```csharp
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Geocoding.Google;

CancellationToken cancellationToken = default;
GoogleGeocoder geocoder = new("your-google-api-key");
IEnumerable<GoogleAddress> addresses = await geocoder.GeocodeAsync(
    "1600 Pennsylvania Ave NW Washington DC 20500",
    cancellationToken);

string? country = addresses
    .Where(address => !address.IsPartialMatch)
    .Select(address => address[GoogleAddressType.Country]?.LongName)
    .FirstOrDefault();
```

## Signed Google Business Credentials

Use signed Google business credentials only when you already rely on that legacy deployment model.

```csharp
using Geocoding.Google;

BusinessKey businessKey = new(
    "your-client-id",
    "your-url-signing-key");

GoogleGeocoder geocoder = new(businessKey);
```

## Build from Source

```bash
dotnet restore
dotnet build Geocoding.slnx
```

## Credentials

- Google Maps: API key, with `BusinessKey` retained only for signed-client compatibility.
- Azure Maps: subscription key.
- Bing Maps: enterprise key for deprecated compatibility scenarios.
- HERE: API key for the current Geocoding and Search API.
- MapQuest: developer API key.
- Yahoo: legacy OAuth consumer key and secret.

Continue with [Provider Support](./providers) for credential setup links, provider-specific notes, and migration guidance.
