using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Geocoding.Here;

/// <summary>
/// Provides geocoding and reverse geocoding through the HERE Geocoding and Search API.
/// </summary>
/// <remarks>
/// https://www.here.com/docs/category/geocoding-search
/// </remarks>
public class HereGeocoder : IGeocoder
{
    private const string BaseAddress = "https://geocode.search.hereapi.com/v1/geocode";
    private const string ReverseBaseAddress = "https://revgeocode.search.hereapi.com/v1/revgeocode";

    private readonly string _apiKey;

    /// <summary>
    /// Gets or sets the proxy used for HERE requests.
    /// </summary>
    public IWebProxy? Proxy { get; set; }
    /// <summary>
    /// Gets or sets the user location bias for requests.
    /// </summary>
    public Location? UserLocation { get; set; }
    /// <summary>
    /// Gets or sets the map view bias for requests.
    /// </summary>
    public Bounds? UserMapView { get; set; }
    /// <summary>
    /// Gets or sets the maximum number of results to request.
    /// </summary>
    public int? MaxResults { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HereGeocoder"/> class.
    /// </summary>
    /// <param name="apiKey">The HERE API key.</param>
    public HereGeocoder(string apiKey)
    {
        if (String.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("apiKey can not be null or empty.", nameof(apiKey));

        _apiKey = apiKey;
    }

    private Uri GetQueryUrl(string address)
    {
        var parameters = CreateBaseParameters();
        parameters.Add(new KeyValuePair<string, string>("q", address));
        AppendGlobalParameters(parameters, includeAtBias: true);
        return BuildUri(BaseAddress, parameters);
    }

    private Uri GetQueryUrl(string street, string city, string state, string postalCode, string country)
    {
        var query = String.Join(", ", new[] { street, city, state, postalCode, country }
            .Where(part => !String.IsNullOrWhiteSpace(part)));

        if (String.IsNullOrWhiteSpace(query))
            throw new ArgumentException("At least one address component is required.");

        return GetQueryUrl(query);
    }

    private Uri GetQueryUrl(double latitude, double longitude)
    {
        var parameters = CreateBaseParameters();
        parameters.Add(new KeyValuePair<string, string>("at", String.Format(CultureInfo.InvariantCulture, "{0},{1}", latitude, longitude)));
        AppendGlobalParameters(parameters, includeAtBias: false);
        return BuildUri(ReverseBaseAddress, parameters);
    }

    private List<KeyValuePair<string, string>> CreateBaseParameters()
    {
        var parameters = new List<KeyValuePair<string, string>>
        {
            new("apiKey", _apiKey!)
        };

        if (MaxResults is not null && MaxResults.Value > 0)
            parameters.Add(new KeyValuePair<string, string>("limit", MaxResults.Value.ToString(CultureInfo.InvariantCulture)));

        return parameters;
    }

    private void AppendGlobalParameters(ICollection<KeyValuePair<string, string>> parameters, bool includeAtBias)
    {
        if (includeAtBias && UserLocation is not null)
            parameters.Add(new KeyValuePair<string, string>("at", String.Format(CultureInfo.InvariantCulture, "{0},{1}", UserLocation.Latitude, UserLocation.Longitude)));

        if (UserMapView is not null)
        {
            parameters.Add(new KeyValuePair<string, string>(
                "in",
                String.Format(
                    CultureInfo.InvariantCulture,
                    "bbox:{0},{1},{2},{3}",
                    UserMapView.SouthWest.Longitude,
                    UserMapView.SouthWest.Latitude,
                    UserMapView.NorthEast.Longitude,
                    UserMapView.NorthEast.Latitude)));
        }
    }

    private Uri BuildUri(string baseAddress, IEnumerable<KeyValuePair<string, string>> parameters)
    {
        var builder = new UriBuilder(baseAddress)
        {
            Query = BuildQueryString(parameters)
        };

        return builder.Uri;
    }

    private string BuildQueryString(IEnumerable<KeyValuePair<string, string>> parameters)
    {
        var builder = new StringBuilder();
        foreach (var pair in parameters)
        {
            if (builder.Length > 0) builder.Append("&");

            builder.Append(UrlEncode(pair.Key));
            builder.Append("=");
            builder.Append(UrlEncode(pair.Value));
        }
        return builder.ToString();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<HereAddress>> GeocodeAsync(string address, CancellationToken cancellationToken = default(CancellationToken))
    {
        if (String.IsNullOrWhiteSpace(address))
            throw new ArgumentException("address can not be null or empty.", nameof(address));

        try
        {
            var url = GetQueryUrl(address);
            var response = await GetResponse(url, cancellationToken).ConfigureAwait(false);
            return ParseResponse(response);
        }
        catch (Exception ex) when (ex is not HereGeocodingException)
        {
            throw new HereGeocodingException(ex);
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<HereAddress>> GeocodeAsync(string street, string city, string state, string postalCode, string country, CancellationToken cancellationToken = default(CancellationToken))
    {
        try
        {
            var url = GetQueryUrl(street, city, state, postalCode, country);
            var response = await GetResponse(url, cancellationToken).ConfigureAwait(false);
            return ParseResponse(response);
        }
        catch (Exception ex) when (ex is not HereGeocodingException)
        {
            throw new HereGeocodingException(ex);
        }
    }

    /// <inheritdoc />
    public Task<IEnumerable<HereAddress>> ReverseGeocodeAsync(Location location, CancellationToken cancellationToken = default(CancellationToken))
    {
        if (location is null)
            throw new ArgumentNullException(nameof(location));

        return ReverseGeocodeAsync(location.Latitude, location.Longitude, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<HereAddress>> ReverseGeocodeAsync(double latitude, double longitude, CancellationToken cancellationToken = default(CancellationToken))
    {
        try
        {
            var url = GetQueryUrl(latitude, longitude);
            var response = await GetResponse(url, cancellationToken).ConfigureAwait(false);
            return ParseResponse(response);
        }
        catch (Exception ex) when (ex is not HereGeocodingException)
        {
            throw new HereGeocodingException(ex);
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

    private IEnumerable<HereAddress> ParseResponse(HereResponse response)
    {
        if (response.Items is null)
            yield break;

        foreach (var item in response.Items)
        {
            if (item?.Position is null)
                continue;

            var address = item.Address ?? new HereAddressPayload();
            var coordinates = item.Access?.FirstOrDefault() ?? item.Position;
            yield return new HereAddress(
                address.Label ?? item.Title ?? "",
                new Location(coordinates.Lat, coordinates.Lng),
                address.Street,
                address.HouseNumber,
                address.City ?? address.County,
                address.State ?? address.StateCode,
                address.PostalCode,
                address.CountryName ?? address.CountryCode,
                MapLocationType(item.ResultType));
        }
    }

    private HttpRequestMessage CreateRequest(Uri url)
    {
        return new HttpRequestMessage(HttpMethod.Get, url);
    }

    private HttpClient BuildClient()
    {
        if (Proxy is null)
            return new HttpClient();

        var handler = new HttpClientHandler { Proxy = Proxy };
        return new HttpClient(handler);
    }

    private async Task<HereResponse> GetResponse(Uri queryUrl, CancellationToken cancellationToken)
    {
        using var client = BuildClient();
        using var request = CreateRequest(queryUrl);
        using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new HereGeocodingException($"HERE request failed ({(int)response.StatusCode} {response.ReasonPhrase}): {json}", response.ReasonPhrase, ((int)response.StatusCode).ToString(CultureInfo.InvariantCulture));

        return JsonSerializer.Deserialize<HereResponse>(json, Extensions.JsonOptions) ?? new HereResponse();
    }

    private static HereLocationType MapLocationType(string? resultType)
    {
        switch (resultType?.Trim().ToLowerInvariant())
        {
            case "housenumber":
            case "street":
            case "addressblock":
            case "intersection":
                return HereLocationType.Address;
            case "place":
                return HereLocationType.Point;
            case "locality":
            case "district":
            case "postalcode":
            case "county":
            case "state":
            case "administrativearea":
            case "country":
                return HereLocationType.Area;
            default:
                return HereLocationType.Unknown;
        }
    }

    private string UrlEncode(string toEncode)
    {
        if (String.IsNullOrEmpty(toEncode))
            return String.Empty;

        return WebUtility.UrlEncode(toEncode);
    }

    private sealed class HereResponse
    {
        [JsonPropertyName("items")]
        public HereItem[] Items { get; set; } = Array.Empty<HereItem>();
    }

    private sealed class HereItem
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("resultType")]
        public string? ResultType { get; set; }

        [JsonPropertyName("address")]
        public HereAddressPayload? Address { get; set; }

        [JsonPropertyName("position")]
        public HerePosition? Position { get; set; }

        [JsonPropertyName("access")]
        public HerePosition[]? Access { get; set; }
    }

    private sealed class HereAddressPayload
    {
        [JsonPropertyName("label")]
        public string? Label { get; set; }

        [JsonPropertyName("houseNumber")]
        public string? HouseNumber { get; set; }

        [JsonPropertyName("street")]
        public string? Street { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("county")]
        public string? County { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonPropertyName("stateCode")]
        public string? StateCode { get; set; }

        [JsonPropertyName("postalCode")]
        public string? PostalCode { get; set; }

        [JsonPropertyName("countryCode")]
        public string? CountryCode { get; set; }

        [JsonPropertyName("countryName")]
        public string? CountryName { get; set; }
    }

    private sealed class HerePosition
    {
        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lng")]
        public double Lng { get; set; }
    }
}
