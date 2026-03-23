# Provider Support

Geocoding.net keeps the calling model consistent across providers, but each service has different setup requirements, lifecycle status, and operational tradeoffs.

## Support Matrix

| Provider | Package | Status | Auth | Notes |
| --- | --- | --- | --- | --- |
| Google Maps | `Geocoding.Google` | Supported | API key or signed client credentials | Strong default when you already operate on Google Cloud. |
| Azure Maps | `Geocoding.Microsoft` | Supported | Azure Maps subscription key | Preferred Microsoft-backed provider for new integrations. |
| Bing Maps | `Geocoding.Microsoft` | Compatibility only | Bing Maps enterprise key | Keep only while migrating existing consumers to Azure Maps. |
| HERE Geocoding and Search | `Geocoding.Here` | Supported | HERE API key | Current HERE Geocoding and Search API support. |
| MapQuest | `Geocoding.MapQuest` | Supported | API key | Commercial MapQuest API only. OpenStreetMap mode is retired. |
| Yahoo PlaceFinder/BOSS | `Geocoding.Yahoo` | Compatibility only | OAuth consumer key and secret | Legacy package retained for controlled retirement work. |

## Choose a Provider

- Choose Azure Maps when you want the primary Microsoft-backed path for new development.
- Choose Google Maps when you need Google's provider-specific result model or existing Google Cloud operations.
- Choose HERE or MapQuest when those services are already part of your data, billing, or compliance boundary.
- Keep Bing Maps and Yahoo only for compatibility and migration work.

## Provider Guides

- [Google Maps](./providers/google)
- [Azure Maps](./providers/azure-maps)
- [HERE provider guide](./providers/here)
- [MapQuest](./providers/mapquest)
- [Bing Maps Compatibility](./providers/bing-maps)
- [Yahoo Compatibility](./providers/yahoo)

## Integration Checklist

1. Install the provider package you actually need.
2. Follow the provider guide to create credentials from the official vendor portal.
3. Instantiate the matching geocoder type directly in your application wiring.
4. Use provider-specific address types only when you need provider-only fields.
5. Decide early whether you are starting greenfield or migrating a compatibility provider off an older service.

## Migration Notes

- Bing Maps remains in the repo for existing enterprise consumers, but Azure Maps is the forward path.
- Yahoo remains a compatibility surface only; plan to remove it from production workflows.
- `BusinessKey` is retained for Google signed-client compatibility, but new Google integrations should use standard API keys.
