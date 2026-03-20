# Generic C# Geocoding API [![CI](https://github.com/exceptionless/Geocoding.net/actions/workflows/build.yml/badge.svg)](https://github.com/exceptionless/Geocoding.net/actions/workflows/build.yml) [![Publish Packages](https://github.com/exceptionless/Geocoding.net/actions/workflows/publish-packages.yml/badge.svg)](https://github.com/exceptionless/Geocoding.net/actions/workflows/publish-packages.yml) [![CodeQL](https://github.com/exceptionless/Geocoding.net/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/exceptionless/Geocoding.net/actions/workflows/codeql-analysis.yml)

Includes a model and interface for communicating with five popular Geocoding providers.  Current implementations include:

* [Google Maps](https://developers.google.com/maps/) - [Google geocoding docs](https://developers.google.com/maps/documentation/geocoding/)
* [Yahoo! BOSS Geo Services](http://developer.yahoo.com/boss/geo/) - [Yahoo PlaceFinder docs](http://developer.yahoo.com/geo/placefinder/guide/index.html)
* [Bing Maps (aka Virtual Earth)](http://www.microsoft.com/maps/) - [Bing geocoding docs](http://msdn.microsoft.com/en-us/library/ff701715.aspx)
* :warning: MapQuest [(Commercial API)](http://www.mapquestapi.com/) - [MapQuest geocoding docs](http://www.mapquestapi.com/geocoding/)
* :warning: MapQuest [(OpenStreetMap)](http://open.mapquestapi.com/) - [MapQuest OpenStreetMap geocoding docs](http://open.mapquestapi.com/geocoding/)
* [HERE Maps](https://www.here.com/) - [HERE developer documentation](https://developer.here.com/documentation)

The API returns latitude/longitude coordinates and normalized address information.  This can be used to perform address validation, real time mapping of user-entered addresses, distance calculations, and much more.

See latest [release notes](https://github.com/exceptionless/Geocoding.net/releases/latest).

:warning: There is a potential issue ([#29](https://github.com/chadly/Geocoding.net/issues/29)) regarding MapQuest that has a workaround. If you would like to help fix the issue, PRs are welcome.

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
Install-Package Geocoding.Yahoo
Install-Package Geocoding.Here
```

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
IGeocoder geocoder = new YahooGeocoder("consumer-key", "consumer-secret");
IEnumerable<Address> addresses = await geocoder.ReverseGeocodeAsync(38.8976777, -77.036517);
```

### Using Provider-Specific Data

```csharp
GoogleGeocoder geocoder = new GoogleGeocoder();
IEnumerable<GoogleAddress> addresses = await geocoder.GeocodeAsync("1600 pennsylvania ave washington dc");

var country = addresses.Where(a => !a.IsPartialMatch).Select(a => a[GoogleAddressType.Country]).First();
Console.WriteLine("Country: " + country.LongName + ", " + country.ShortName); //Country: United States, US
```

The Microsoft and Yahoo implementations each provide their own address class as well, `BingAddress` and `YahooAddress`.

## API Keys

Google [requires a new Server API Key](https://developers.google.com/maps/documentation/javascript/tutorial#api_key) to access its service.

Bing [requires an API key](http://msdn.microsoft.com/en-us/library/ff428642.aspx) to access its service.

You will need a [consumer secret and consumer key](http://developer.yahoo.com/boss/geo/BOSS_Signup.pdf) (PDF) for Yahoo.

MapQuest API requires a key. Sign up here: (<http://developer.mapquest.com/web/products/open>)

HERE requires an [app ID and app Code](https://developer.here.com/?create=Freemium-Basic&keepState=true&step=account)

## How to Build from Source

```bash
dotnet restore
dotnet build
```

For a nice experience, use [Visual Studio Code](https://code.visualstudio.com/) to work with the project. The editor is cross platform and open source.

Alternatively, if you are on Windows, you can open the solution in [Visual Studio](https://www.visualstudio.com/) and build.

### Service Tests

You will need to generate API keys for each respective service to run the service tests. Make a `settings-override.json` as a copy of `settings.json` in the test project and put in your API keys. Then you should be able to run the tests.

Most provider-backed integration tests skip with a message indicating which setting is required when credentials are missing. The Yahoo suite is still explicitly skipped while issue #27 remains open, but it now uses the same credential checks when those tests are re-enabled.

## Sample App

The sample app in `samples/Example.Web` is an ASP.NET Core 10 minimal API that can geocode and reverse geocode against any configured provider.

```bash
dotnet run --project samples/Example.Web/Example.Web.csproj
```

Configure a provider in `samples/Example.Web/appsettings.json` or via environment variables such as `Providers__Google__ApiKey`. Once the app is running, use `samples/Example.Web/sample.http` to call `/providers`, `/geocode`, and `/reverse`.
