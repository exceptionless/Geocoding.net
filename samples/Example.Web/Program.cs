using Geocoding;
using Geocoding.Google;
using Geocoding.Here;
using Geocoding.MapQuest;
using Geocoding.Microsoft;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ProviderOptions>(builder.Configuration.GetSection("Providers"));

var app = builder.Build();

app.MapGet("/", (IOptions<ProviderOptions> options) => Results.Ok(new
{
    sample = "Geocoding.net minimal API sample",
    endpoints = new[]
    {
        "/providers",
        "/geocode?provider=google&address=1600 Pennsylvania Ave NW, Washington, DC",
        "/reverse?provider=google&latitude=38.8976763&longitude=-77.0365298"
    },
    configuredProviders = GetConfiguredProviders(options.Value)
}));

app.MapGet("/providers", (IOptions<ProviderOptions> options) => Results.Ok(GetConfiguredProviders(options.Value)));

app.MapGet("/geocode", async Task<IResult> (string? provider, string? address, IOptions<ProviderOptions> options, CancellationToken cancellationToken) =>
{
    var validationErrors = new List<(string Key, string Error)>();

    if (String.IsNullOrWhiteSpace(provider))
        validationErrors.Add((nameof(provider), "A provider is required."));

    if (String.IsNullOrWhiteSpace(address))
        validationErrors.Add((nameof(address), "An address is required."));

    if (validationErrors.Count > 0)
        return ValidationProblem(validationErrors.ToArray());

    var providerValue = provider!;
    var addressValue = address!;

    if (!TryCreateGeocoder(providerValue, options.Value, out var geocoder, out var error))
        return ValidationProblem((nameof(provider), error!));

    try
    {
        var results = await geocoder.GeocodeAsync(addressValue, cancellationToken).ConfigureAwait(false);

        return Results.Ok(new GeocodeResponse(providerValue, addressValue, results.Select(MapAddress).ToArray()));
    }
    catch (Geocoding.Core.GeocodingException exception)
    {
        return ProviderProblem(providerValue, options.Value, exception);
    }
});

app.MapGet("/reverse", async Task<IResult> (string? provider, double? latitude, double? longitude, IOptions<ProviderOptions> options, CancellationToken cancellationToken) =>
{
    var validationErrors = new List<(string Key, string Error)>();

    if (String.IsNullOrWhiteSpace(provider))
        validationErrors.Add((nameof(provider), "A provider is required."));

    if (latitude is null)
        validationErrors.Add((nameof(latitude), "A latitude is required."));
    else if (Double.IsNaN(latitude.Value))
        validationErrors.Add((nameof(latitude), "Latitude must be a valid number."));
    else if (latitude.Value < -90 || latitude.Value > 90)
        validationErrors.Add((nameof(latitude), "Latitude must be between -90 and 90 inclusive."));

    if (longitude is null)
        validationErrors.Add((nameof(longitude), "A longitude is required."));
    else if (Double.IsNaN(longitude.Value))
        validationErrors.Add((nameof(longitude), "Longitude must be a valid number."));
    else if (longitude.Value < -180 || longitude.Value > 180)
        validationErrors.Add((nameof(longitude), "Longitude must be between -180 and 180 inclusive."));

    if (validationErrors.Count > 0)
        return ValidationProblem(validationErrors.ToArray());

    var providerValue = provider!;
    var latitudeValue = latitude!.Value;
    var longitudeValue = longitude!.Value;

    if (!TryCreateGeocoder(providerValue, options.Value, out var geocoder, out var error))
        return ValidationProblem((nameof(provider), error!));

    try
    {
        var results = await geocoder.ReverseGeocodeAsync(latitudeValue, longitudeValue, cancellationToken).ConfigureAwait(false);

        return Results.Ok(new ReverseGeocodeResponse(providerValue, latitudeValue, longitudeValue, results.Select(MapAddress).ToArray()));
    }
    catch (ArgumentOutOfRangeException exception)
    {
        return ValidationProblem((exception.ParamName ?? nameof(latitude), exception.Message));
    }
    catch (Geocoding.Core.GeocodingException exception)
    {
        return ProviderProblem(providerValue, options.Value, exception);
    }
});

app.Run();

static string[] GetConfiguredProviders(ProviderOptions options)
{
    var configuredProviders = new List<string>();

    if (!String.IsNullOrWhiteSpace(options.Azure.ApiKey))
        configuredProviders.Add("azure");

    if (!String.IsNullOrWhiteSpace(options.Bing.ApiKey))
        configuredProviders.Add("bing");

    configuredProviders.Add("google");

    if (!String.IsNullOrWhiteSpace(options.Here.ApiKey))
        configuredProviders.Add("here");

    if (!String.IsNullOrWhiteSpace(options.MapQuest.ApiKey))
        configuredProviders.Add("mapquest");

    return configuredProviders.ToArray();
}

static bool TryCreateGeocoder(string provider, ProviderOptions options, out IGeocoder geocoder, out string? error)
{
    switch (provider.Trim().ToLowerInvariant())
    {
        case "google":
            geocoder = String.IsNullOrWhiteSpace(options.Google.ApiKey)
                ? new GoogleGeocoder()
                : new GoogleGeocoder(options.Google.ApiKey);
            error = null;
            return true;

        case "azure":
            if (String.IsNullOrWhiteSpace(options.Azure.ApiKey))
            {
                geocoder = default!;
                error = "Configure Providers:Azure:ApiKey before using the Azure Maps provider.";
                return false;
            }

            geocoder = new AzureMapsGeocoder(options.Azure.ApiKey);
            error = null;
            return true;

        case "bing":
            if (String.IsNullOrWhiteSpace(options.Bing.ApiKey))
            {
                geocoder = default!;
                error = "Configure Providers:Bing:ApiKey before using the Bing provider.";
                return false;
            }

            geocoder = new BingMapsGeocoder(options.Bing.ApiKey);
            error = null;
            return true;

        case "here":
            geocoder = default!;
            if (String.IsNullOrWhiteSpace(options.Here.ApiKey))
            {
                error = "Configure Providers:Here:ApiKey before using the HERE provider.";
                return false;
            }

            geocoder = new HereGeocoder(options.Here.ApiKey);
            error = null;
            return true;

        case "mapquest":
            if (String.IsNullOrWhiteSpace(options.MapQuest.ApiKey))
            {
                geocoder = default!;
                error = "Configure Providers:MapQuest:ApiKey before using the MapQuest provider.";
                return false;
            }

            if (options.MapQuest.UseOsm)
            {
                geocoder = default!;
                error = "MapQuest OpenStreetMap mode is no longer supported. Use the commercial MapQuest API instead.";
                return false;
            }

            geocoder = new MapQuestGeocoder(options.MapQuest.ApiKey)
            {
                UseOSM = options.MapQuest.UseOsm
            };
            error = null;
            return true;

        default:
            geocoder = default!;
            error = $"Unknown provider '{provider}'. Use one of: azure, bing, google, here, mapquest.";
            return false;
    }
}

static AddressResponse MapAddress(Address address) =>
    new(address.FormattedAddress, address.Provider, address.Coordinates.Latitude, address.Coordinates.Longitude);

static IResult ValidationProblem(params (string Key, string Error)[] errors)
{
    var dictionary = errors
        .Where(error => !String.IsNullOrWhiteSpace(error.Error))
        .GroupBy(error => error.Key)
        .ToDictionary(group => group.Key, group => group.Select(error => error.Error).ToArray(), StringComparer.OrdinalIgnoreCase);

    return Results.ValidationProblem(dictionary);
}

static IResult ProviderProblem(string provider, ProviderOptions options, Exception exception)
{
    var detail = exception.Message;

    if (String.Equals(provider, "google", StringComparison.OrdinalIgnoreCase) && String.IsNullOrWhiteSpace(options.Google.ApiKey))
        detail = "Google rejected the request. Configure Providers:Google:ApiKey if your environment requires an API key.";

    return Results.Problem(title: "Geocoding provider request failed.", detail: detail, statusCode: StatusCodes.Status502BadGateway);
}

internal sealed record GeocodeResponse(string Provider, string Address, AddressResponse[] Results);

internal sealed record ReverseGeocodeResponse(string Provider, double Latitude, double Longitude, AddressResponse[] Results);

internal sealed record AddressResponse(string FormattedAddress, string Provider, double Latitude, double Longitude);

internal sealed class ProviderOptions
{
    public AzureProviderOptions Azure { get; init; } = new();
    public BingProviderOptions Bing { get; init; } = new();
    public GoogleProviderOptions Google { get; init; } = new();
    public HereProviderOptions Here { get; init; } = new();
    public MapQuestProviderOptions MapQuest { get; init; } = new();
}

internal sealed class AzureProviderOptions
{
    public String ApiKey { get; init; } = String.Empty;
}

internal sealed class BingProviderOptions
{
    public String ApiKey { get; init; } = String.Empty;
}

internal sealed class GoogleProviderOptions
{
    public String ApiKey { get; init; } = String.Empty;
}

internal sealed class HereProviderOptions
{
    public String ApiKey { get; init; } = String.Empty;
}

internal sealed class MapQuestProviderOptions
{
    public String ApiKey { get; init; } = String.Empty;
    public bool UseOsm { get; init; }
}
