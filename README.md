# Generic C# Geocoding API [![CI](https://github.com/exceptionless/Geocoding.net/actions/workflows/build.yml/badge.svg)](https://github.com/exceptionless/Geocoding.net/actions/workflows/build.yml) [![CodeQL](https://github.com/exceptionless/Geocoding.net/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/exceptionless/Geocoding.net/actions/workflows/codeql-analysis.yml)

Includes a model and interface for communicating with current geocoding providers while preserving selected legacy compatibility surfaces.

| Provider | Package | Status | Auth | Notes |
| --- | --- | --- | --- | --- |
| Google Maps | `Geocoding.Google` | Supported | API key or signed client credentials | `BusinessKey` supports signed Google Maps client-based requests when your deployment requires them. |
| Azure Maps | `Geocoding.Microsoft` | Supported | Azure Maps subscription key | Primary Microsoft-backed geocoder. |
| Bing Maps | `Geocoding.Microsoft` | Deprecated compatibility | Bing Maps enterprise key | `BingMapsGeocoder` remains available for existing consumers and is marked obsolete for new development. |
| HERE Geocoding and Search | `Geocoding.Here` | Supported | HERE API key | Uses the current HERE Geocoding and Search API. |
| MapQuest | `Geocoding.MapQuest` | Supported | API key | Commercial API only. OpenStreetMap mode is no longer supported. |
| Yahoo PlaceFinder/BOSS | `Geocoding.Yahoo` | Deprecated | None verified | Legacy package retained only for source compatibility and planned removal. |

The API returns latitude/longitude coordinates and normalized address information.  This can be used to perform address validation, real time mapping of user-entered addresses, distance calculations, and much more.

See latest [release notes](https://github.com/exceptionless/Geocoding.net/releases/latest).

:warning: MapQuest OpenStreetMap mode was tied to a retired service surface and now fails fast instead of silently calling dead endpoints.

## Installation

Install [via nuget](http://www.nuget.org/packages/Geocoding.net/):

```bash
Install-Package Geocoding.Core
```

and then choose which provider you want to install (or install all of them):

```bash
Install-Package Geocoding.Google
Install-Package Geocoding.MapQuest
Install-Package Geocoding.Microsoft
Install-Package Geocoding.Here
```

If you still need the deprecated Yahoo compatibility package, install `Geocoding.Yahoo` explicitly and plan to remove it before the next major version.

## Example Usage

### Simple Example

```csharp
IGeocoder geocoder = new GoogleGeocoder() { ApiKey = "this-is-my-google-api-key" };
IEnumerable<Address> addresses = await geocoder.GeocodeAsync("1600 pennsylvania ave washington dc");
Console.WriteLine("Formatted: " + addresses.First().FormattedAddress); //Formatted: 1600 Pennsylvania Ave SE, Washington, DC 20003, USA
Console.WriteLine("Coordinates: " + addresses.First().Coordinates.Latitude + ", " + addresses.First().Coordinates.Longitude); //Coordinates: 38.8791981, -76.9818437
```

It can also be used to return address information from latitude/longitude coordinates (aka reverse geocoding):

```csharp
IGeocoder geocoder = new AzureMapsGeocoder("this-is-my-azure-maps-key");
IEnumerable<Address> addresses = await geocoder.ReverseGeocodeAsync(38.8976777, -77.036517);
```

### Using Provider-Specific Data

```csharp
GoogleGeocoder geocoder = new GoogleGeocoder();
IEnumerable<GoogleAddress> addresses = await geocoder.GeocodeAsync("1600 pennsylvania ave washington dc");

var country = addresses.Where(a => !a.IsPartialMatch).Select(a => a[GoogleAddressType.Country]).First();
Console.WriteLine("Country: " + country.LongName + ", " + country.ShortName); //Country: United States, US
```

The Microsoft providers expose `AzureMapsAddress`, and the legacy `BingMapsGeocoder` / `BingAddress` surface remains available as an obsolete compatibility layer. The Yahoo package remains deprecated.

## API Keys

Google uses a [Geocoding API key](https://developers.google.com/maps/documentation/geocoding/get-api-key), and many environments now require one for reliable access.

Azure Maps requires an [Azure Maps account key](https://learn.microsoft.com/en-us/azure/azure-maps/how-to-manage-account-keys#create-a-new-account).

Bing Maps requires an existing Bing Maps enterprise key. The provider is deprecated and retained only for compatibility during migration to Azure Maps.

MapQuest requires a [developer API key](https://developer.mapquest.com/user/me/apps).

HERE requires a [HERE API key](https://www.here.com/docs/category/identity-and-access-management).

Yahoo credential onboarding could not be validated and the package is deprecated.

## How to Build from Source

```bash
dotnet restore
dotnet build
```

For a nice experience, use [Visual Studio Code](https://code.visualstudio.com/) to work with the project. The editor is cross platform and open source.

Alternatively, if you are on Windows, you can open the solution in [Visual Studio](https://www.visualstudio.com/) and build.

### Service Tests

You will need to generate API keys for each respective service to run the service tests. Make a `settings-override.json` as a copy of `settings.json` in the test project and put in your API keys. Then you should be able to run the tests.

Most provider-backed integration tests skip with a message indicating which setting is required when credentials are missing. The Yahoo suite remains explicitly skipped while the provider is deprecated.

## Sample App

The sample app in `samples/Example.Web` is an ASP.NET Core 10 minimal API that can geocode and reverse geocode against any configured provider, including the deprecated Bing compatibility option when explicitly enabled.

```bash
dotnet run --project samples/Example.Web/Example.Web.csproj
```

Configure a provider in `samples/Example.Web/appsettings.json` or via environment variables such as `Providers__Google__ApiKey`, `Providers__Azure__ApiKey`, `Providers__Bing__ApiKey`, `Providers__Here__ApiKey`, or `Providers__MapQuest__ApiKey`. Once the app is running, use `samples/Example.Web/sample.http` to call `/providers`, `/geocode`, and `/reverse`.
