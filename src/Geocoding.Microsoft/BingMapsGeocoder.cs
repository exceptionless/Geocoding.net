using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Geocoding.Microsoft;

/// <summary>
/// Provides geocoding and reverse geocoding through the Bing Maps API.
/// </summary>
/// <remarks>
/// New development should prefer <see cref="AzureMapsGeocoder"/>. Bing Maps remains available for existing enterprise consumers only.
/// </remarks>
public class BingMapsGeocoder : IGeocoder
{
    private const string UnformattedQuery = "https://dev.virtualearth.net/REST/v1/Locations/{0}?key={1}";
    private const string FormattedQuery = "https://dev.virtualearth.net/REST/v1/Locations?{0}&key={1}";
    private const string Query = "q={0}";
    private const string Country = "countryRegion={0}";
    private const string Admin = "adminDistrict={0}";
    private const string Zip = "postalCode={0}";
    private const string City = "locality={0}";
    private const string Address = "addressLine={0}";
    private const int Bingmaxresultsvalue = 20;

    private readonly string _bingKey;

    /// <summary>
    /// Gets or sets the proxy used for Bing Maps requests.
    /// </summary>
    public IWebProxy? Proxy { get; set; }
    /// <summary>
    /// Gets or sets the culture used for results.
    /// </summary>
    public string? Culture { get; set; }
    /// <summary>
    /// Gets or sets the user location bias.
    /// </summary>
    public Location? UserLocation { get; set; }
    /// <summary>
    /// Gets or sets the user map view bias.
    /// </summary>
    public Bounds? UserMapView { get; set; }
    /// <summary>
    /// Gets or sets the user IP address sent to Bing Maps.
    /// </summary>
    public IPAddress? UserIP { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether neighborhoods should be included.
    /// </summary>
    public bool IncludeNeighborhood { get; set; }
    /// <summary>
    /// Gets or sets the maximum number of results to request.
    /// </summary>
    public int? MaxResults { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BingMapsGeocoder"/> class.
    /// </summary>
    /// <param name="bingKey">The Bing Maps API key.</param>
    public BingMapsGeocoder(string bingKey)
    {
        if (String.IsNullOrWhiteSpace(bingKey))
            throw new ArgumentException("bingKey can not be null or empty.", nameof(bingKey));

        _bingKey = bingKey;
    }

    private string GetQueryUrl(string address)
    {
        var parameters = new StringBuilder();
        bool first = true;
        first = AppendParameter(parameters, address, Query, first);
        first = AppendGlobalParameters(parameters, first);

        return String.Format(FormattedQuery, parameters.ToString(), _bingKey);
    }

    private string GetQueryUrl(string street, string city, string state, string postalCode, string country)
    {
        StringBuilder parameters = new StringBuilder();
        bool first = true;
        first = AppendParameter(parameters, city, City, first);
        first = AppendParameter(parameters, state, Admin, first);
        first = AppendParameter(parameters, postalCode, Zip, first);
        first = AppendParameter(parameters, country, Country, first);
        first = AppendParameter(parameters, street, Address, first);
        first = AppendGlobalParameters(parameters, first);

        return String.Format(FormattedQuery, parameters.ToString(), _bingKey);
    }

    private string GetQueryUrl(double latitude, double longitude)
    {
        var builder = new StringBuilder(String.Format(UnformattedQuery, String.Format(CultureInfo.InvariantCulture, "{0},{1}", latitude, longitude), _bingKey));
        AppendGlobalParameters(builder, false);
        return builder.ToString();
    }

    private IEnumerable<KeyValuePair<string, string>> GetGlobalParameters()
    {
        if (Culture is { Length: > 0 })
            yield return new KeyValuePair<string, string>("c", Culture);

        if (UserLocation is not null)
            yield return new KeyValuePair<string, string>("userLocation", UserLocation.ToString());

        if (UserMapView is not null)
            yield return new KeyValuePair<string, string>("userMapView", String.Concat(UserMapView.SouthWest.ToString(), ",", UserMapView.NorthEast.ToString()));

        if (UserIP is not null)
            yield return new KeyValuePair<string, string>("userIp", UserIP.ToString());

        if (IncludeNeighborhood)
            yield return new KeyValuePair<string, string>("inclnb", IncludeNeighborhood ? "1" : "0");

        if (MaxResults is not null && MaxResults.Value > 0)
            yield return new KeyValuePair<string, string>("maxResults", Math.Min(MaxResults.Value, Bingmaxresultsvalue).ToString());
    }

    private bool AppendGlobalParameters(StringBuilder parameters, bool first)
    {
        var values = GetGlobalParameters().ToArray();

        if (!first) parameters.Append("&");
        parameters.Append(BuildQueryString(values));

        return first && !values.Any();
    }

    private string BuildQueryString(IEnumerable<KeyValuePair<string, string>> parameters)
    {
        var builder = new StringBuilder();
        foreach (var pair in parameters)
        {
            if (builder.Length > 0) builder.Append("&");

            builder.Append(BingUrlEncode(pair.Key));
            builder.Append("=");
            builder.Append(BingUrlEncode(pair.Value));
        }
        return builder.ToString();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<BingAddress>> GeocodeAsync(string address, CancellationToken cancellationToken = default(CancellationToken))
    {
        try
        {
            var url = GetQueryUrl(address);
            var response = await GetResponse(url, cancellationToken).ConfigureAwait(false);
            return ParseResponse(response);
        }
        catch (Exception ex) when (ex is not BingGeocodingException)
        {
            throw new BingGeocodingException(ex);
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<BingAddress>> GeocodeAsync(string street, string city, string state, string postalCode, string country, CancellationToken cancellationToken = default(CancellationToken))
    {
        try
        {
            var url = GetQueryUrl(street, city, state, postalCode, country);
            var response = await GetResponse(url, cancellationToken).ConfigureAwait(false);
            return ParseResponse(response);
        }
        catch (Exception ex) when (ex is not BingGeocodingException)
        {
            throw new BingGeocodingException(ex);
        }
    }

    /// <inheritdoc />
    public Task<IEnumerable<BingAddress>> ReverseGeocodeAsync(Location location, CancellationToken cancellationToken = default(CancellationToken))
    {
        if (location is null)
            throw new ArgumentNullException(nameof(location));

        return ReverseGeocodeAsync(location.Latitude, location.Longitude, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<BingAddress>> ReverseGeocodeAsync(double latitude, double longitude, CancellationToken cancellationToken = default(CancellationToken))
    {
        try
        {
            var url = GetQueryUrl(latitude, longitude);
            var response = await GetResponse(url, cancellationToken).ConfigureAwait(false);
            return ParseResponse(response);
        }
        catch (Exception ex) when (ex is not BingGeocodingException)
        {
            throw new BingGeocodingException(ex);
        }
    }

    async Task<IEnumerable<Address>> IGeocoder.GeocodeAsync(string address, CancellationToken cancellationToken)
    {
        return await GeocodeAsync(address, cancellationToken).ConfigureAwait(false);
    }

    async Task<IEnumerable<Address>> IGeocoder.GeocodeAsync(string street, string city, string state, string postalCode, string country, CancellationToken cancellationToken)
    {
        return await GeocodeAsync(street, city, state, postalCode, country, cancellationToken).ConfigureAwait(false);
    }

    async Task<IEnumerable<Address>> IGeocoder.ReverseGeocodeAsync(Location location, CancellationToken cancellationToken)
    {
        return await ReverseGeocodeAsync(location, cancellationToken).ConfigureAwait(false);
    }

    async Task<IEnumerable<Address>> IGeocoder.ReverseGeocodeAsync(double latitude, double longitude, CancellationToken cancellationToken)
    {
        return await ReverseGeocodeAsync(latitude, longitude, cancellationToken).ConfigureAwait(false);
    }

    private bool AppendParameter(StringBuilder sb, string parameter, string format, bool first)
    {
        if (!String.IsNullOrEmpty(parameter))
        {
            if (!first)
            {
                sb.Append('&');
            }
            sb.Append(String.Format(format, BingUrlEncode(parameter)));
            return false;
        }
        return first;
    }

    private IEnumerable<BingAddress> ParseResponse(Json.Response response)
    {
        var list = new List<BingAddress>();

        foreach (Json.Location location in response.ResourceSets[0].Resources)
        {
            if (location.Point is null || location.Address is null)
                continue;

            if (!Enum.TryParse(location.EntityType, out EntityType entityType))
                entityType = EntityType.Unknown;

            list.Add(new BingAddress(
                location.Address.FormattedAddress!,
                new Location(location.Point.Coordinates[0], location.Point.Coordinates[1]),
                location.Address.AddressLine,
                location.Address.AdminDistrict,
                location.Address.AdminDistrict2,
                location.Address.CountryRegion,
                location.Address.Locality,
                location.Address.Neighborhood,
                location.Address.PostalCode,
                entityType,
                EvaluateConfidence(location.Confidence)
            ));
        }

        return list;
    }

    private HttpRequestMessage CreateRequest(string url)
    {
        return new HttpRequestMessage(HttpMethod.Get, url);
    }

    private HttpClient BuildClient()
    {
        if (Proxy is null)
            return new HttpClient();

        var handler = new HttpClientHandler();
        handler.Proxy = Proxy;
        return new HttpClient(handler);
    }

    private async Task<Json.Response> GetResponse(string queryUrl, CancellationToken cancellationToken)
    {
        using (var client = BuildClient())
        {
            var response = await client.SendAsync(CreateRequest(queryUrl), cancellationToken).ConfigureAwait(false);
            using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                return await JsonSerializer.DeserializeAsync<Json.Response>(stream, Extensions.JsonOptions, cancellationToken).ConfigureAwait(false)
                    ?? new Json.Response();
            }
        }
    }

    private ConfidenceLevel EvaluateConfidence(string? confidence)
    {
        switch (confidence?.ToLower())
        {
            case "low":
                return ConfidenceLevel.Low;
            case "medium":
                return ConfidenceLevel.Medium;
            case "high":
                return ConfidenceLevel.High;
            default:
                return ConfidenceLevel.Unknown;
        }
    }

    private string BingUrlEncode(string toEncode)
    {
        if (String.IsNullOrEmpty(toEncode))
            return String.Empty;

        return WebUtility.UrlEncode(toEncode);
    }
}
