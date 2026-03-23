# Azure Maps

## When to Use It

Use `Geocoding.Microsoft` with `AzureMapsGeocoder` for new Microsoft-backed integrations. This is the primary Microsoft provider in the repository.

## Package

```powershell
Install-Package Geocoding.Microsoft
```

## Official References

- [Azure Maps geocoding documentation](https://learn.microsoft.com/azure/azure-maps/how-to-search-for-address)
- [Create and manage Azure Maps account keys](https://learn.microsoft.com/azure/azure-maps/how-to-manage-account-keys#create-a-new-account)
- [Azure portal](https://portal.azure.com/)

## How to Get a Key

1. Create an Azure Maps account in your Azure subscription.
2. Open the Azure Maps account in the Azure portal.
3. Generate or copy a primary or secondary subscription key.
4. Store the key in your app configuration or secret store.

## Minimal Setup

```csharp
using System.Collections.Generic;
using System.Threading;
using Geocoding;
using Geocoding.Microsoft;

CancellationToken cancellationToken = default;
IGeocoder geocoder = new AzureMapsGeocoder("your-azure-maps-key");
IEnumerable<Address> results = await geocoder.ReverseGeocodeAsync(
    38.8976777,
    -77.036517,
    cancellationToken);
```

## Provider-Specific Features

- `AzureMapsAddress` exposes Azure Maps-specific data while keeping the shared `Address` contract available.
- The Azure implementation is the recommended migration target for existing Bing Maps consumers.

## Operational Notes

- Prefer Azure Maps for new Microsoft-backed workloads instead of starting on Bing Maps compatibility.
- Keep Azure subscription key rotation in your standard secret-management workflow.
- Azure Maps failures surface as `AzureMapsGeocodingException`.
