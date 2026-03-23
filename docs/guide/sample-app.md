# Sample App

The sample app in `samples/Example.Web` demonstrates forward and reverse geocoding through a minimal ASP.NET Core application.

## Run the Sample

```bash
dotnet run --project samples/Example.Web/Example.Web.csproj
```

## Configure Providers

Set provider credentials in `samples/Example.Web/appsettings.json` or through environment variables:

- `Providers__Azure__ApiKey`
- `Providers__Bing__ApiKey`
- `Providers__Google__ApiKey`
- `Providers__Here__ApiKey`
- `Providers__MapQuest__ApiKey`
- `Providers__Yahoo__ConsumerKey`
- `Providers__Yahoo__ConsumerSecret`

The sample intentionally excludes Yahoo from runtime provider selection because the Yahoo provider targets a legacy, discontinued service and is maintained only for compatibility with existing integrations; the placeholder settings remain aligned with the shared test configuration shape.

## Example Configuration

```json
{
  "Providers": {
    "Azure": { "ApiKey": "" },
    "Bing": { "ApiKey": "" },
    "Google": { "ApiKey": "" },
    "Here": { "ApiKey": "" },
    "MapQuest": { "ApiKey": "" },
    "Yahoo": {
      "ConsumerKey": "",
      "ConsumerSecret": ""
    }
  }
}
```

## Endpoints

Use `samples/Example.Web/sample.http` to exercise the sample app:

- `/providers`
- `/geocode`
- `/reverse`

## When to Use It

Use the sample app to verify provider wiring, environment configuration, and request flow before you embed the geocoder into your own host application. Do not treat it as the source of truth for provider behavior or shared API design.
