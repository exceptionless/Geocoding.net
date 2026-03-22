using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Geocoding.Microsoft;

/// <summary>
/// Provides geocoding and reverse geocoding through the Azure Maps Search API.
/// </summary>
public class AzureMapsGeocoder : IGeocoder
{
    private const string ApiVersion = "1.0";
    private const string BaseAddress = "https://atlas.microsoft.com/";
    private const int AzureMaxResults = 100;

    private readonly string _apiKey;

    /// <summary>
    /// Gets or sets the proxy used for Azure Maps requests.
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
    /// Gets or sets the user IP address associated with the request.
    /// Retained for API compatibility only. Azure Maps Search does not accept an explicit user-IP hint when using subscription-key authentication, so the value is ignored.
    /// </summary>
    public IPAddress? UserIP { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether neighborhoods should be included when the provider returns them.
    /// </summary>
    public bool IncludeNeighborhood { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of results to request.
    /// </summary>
    public int? MaxResults { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureMapsGeocoder"/> class.
    /// </summary>
    /// <param name="apiKey">The Azure Maps subscription key.</param>
    public AzureMapsGeocoder(string apiKey)
    {
        if (String.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("apiKey can not be null or empty.", nameof(apiKey));

        _apiKey = apiKey;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<AzureMapsAddress>> GeocodeAsync(string address, CancellationToken cancellationToken = default(CancellationToken))
    {
        if (String.IsNullOrWhiteSpace(address))
            throw new ArgumentException("address can not be null or empty.", nameof(address));

        try
        {
            var response = await GetResponseAsync(BuildSearchUri(address), cancellationToken).ConfigureAwait(false);
            return ParseResponse(response);
        }
        catch (Exception ex) when (ex is not AzureMapsGeocodingException)
        {
            throw new AzureMapsGeocodingException(ex);
        }
    }

    /// <inheritdoc />
    public Task<IEnumerable<AzureMapsAddress>> GeocodeAsync(string street, string city, string state, string postalCode, string country, CancellationToken cancellationToken = default(CancellationToken))
    {
        var parts = new[] { street, city, state, postalCode, country }
            .Where(part => !String.IsNullOrWhiteSpace(part))
            .ToArray();

        if (parts.Length == 0)
            throw new ArgumentException("At least one address component is required.");

        return GeocodeAsync(String.Join(", ", parts), cancellationToken);
    }

    /// <inheritdoc />
    public Task<IEnumerable<AzureMapsAddress>> ReverseGeocodeAsync(Location location, CancellationToken cancellationToken = default(CancellationToken))
    {
        if (location is null)
            throw new ArgumentNullException(nameof(location));

        return ReverseGeocodeAsync(location.Latitude, location.Longitude, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<AzureMapsAddress>> ReverseGeocodeAsync(double latitude, double longitude, CancellationToken cancellationToken = default(CancellationToken))
    {
        try
        {
            var response = await GetResponseAsync(BuildReverseUri(latitude, longitude), cancellationToken).ConfigureAwait(false);
            return ParseResponse(response);
        }
        catch (Exception ex) when (ex is not AzureMapsGeocodingException)
        {
            throw new AzureMapsGeocodingException(ex);
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

    private Uri BuildSearchUri(string query)
    {
        var parameters = CreateBaseParameters();
        parameters.Add(new KeyValuePair<string, string>("query", query));
        AppendSearchBias(parameters);
        return BuildUri("search/address/json", parameters);
    }

    private Uri BuildReverseUri(double latitude, double longitude)
    {
        var parameters = CreateBaseParameters();
        parameters.Add(new KeyValuePair<string, string>("query", String.Format(CultureInfo.InvariantCulture, "{0},{1}", latitude, longitude)));
        return BuildUri("search/address/reverse/json", parameters);
    }

    private List<KeyValuePair<string, string>> CreateBaseParameters()
    {
        var parameters = new List<KeyValuePair<string, string>>
        {
            new("api-version", ApiVersion),
            new("subscription-key", _apiKey)
        };

        if (Culture is { Length: > 0 })
            parameters.Add(new KeyValuePair<string, string>("language", Culture));

        if (MaxResults is > 0)
            parameters.Add(new KeyValuePair<string, string>("limit", Math.Min(MaxResults.Value, AzureMaxResults).ToString(CultureInfo.InvariantCulture)));

        return parameters;
    }

    private void AppendSearchBias(List<KeyValuePair<string, string>> parameters)
    {
        if (UserLocation is not null)
        {
            parameters.Add(new KeyValuePair<string, string>("lat", UserLocation.Latitude.ToString(CultureInfo.InvariantCulture)));
            parameters.Add(new KeyValuePair<string, string>("lon", UserLocation.Longitude.ToString(CultureInfo.InvariantCulture)));
        }

        if (UserMapView is not null)
        {
            parameters.Add(new KeyValuePair<string, string>("topLeft", String.Format(CultureInfo.InvariantCulture, "{0},{1}", UserMapView.NorthEast.Latitude, UserMapView.SouthWest.Longitude)));
            parameters.Add(new KeyValuePair<string, string>("btmRight", String.Format(CultureInfo.InvariantCulture, "{0},{1}", UserMapView.SouthWest.Latitude, UserMapView.NorthEast.Longitude)));
        }
    }

    private Uri BuildUri(string relativePath, IEnumerable<KeyValuePair<string, string>> parameters)
    {
        var builder = new UriBuilder(new Uri(new Uri(BaseAddress), relativePath))
        {
            Query = String.Join("&", parameters.Select(pair => $"{WebUtility.UrlEncode(pair.Key)}={WebUtility.UrlEncode(pair.Value)}"))
        };

        return builder.Uri;
    }

    private async Task<AzureSearchResponse> GetResponseAsync(Uri queryUrl, CancellationToken cancellationToken)
    {
        using (var client = BuildClient())
        using (var request = new HttpRequestMessage(HttpMethod.Get, queryUrl))
        using (var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false))
        {
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new AzureMapsGeocodingException($"Azure Maps request failed ({(int)response.StatusCode} {response.ReasonPhrase}): {json}");

            var payload = JsonSerializer.Deserialize<AzureSearchResponse>(json, Extensions.JsonOptions);
            return payload ?? new AzureSearchResponse();
        }
    }

    private HttpClient BuildClient()
    {
        if (Proxy is null)
            return new HttpClient();

        var handler = new HttpClientHandler { Proxy = Proxy };
        return new HttpClient(handler);
    }

    private IEnumerable<AzureMapsAddress> ParseResponse(AzureSearchResponse response)
    {
        if (response.Results is not null && response.Results.Length > 0)
        {
            foreach (var result in response.Results)
            {
                if (result?.Position is null)
                    continue;

                var address = result.Address ?? new AzureAddressPayload();
                var formattedAddress = FirstNonEmpty(address.FreeformAddress, address.StreetNameAndNumber, BuildStreetLine(address.StreetNumber, address.StreetName), result.Poi?.Name, result.Type, FirstNonEmpty(address.LocalName, address.Municipality, address.CountryTertiarySubdivision), address.Country);
                if (String.IsNullOrWhiteSpace(formattedAddress))
                    continue;

                var locality = FirstNonEmpty(address.LocalName, address.Municipality, address.CountryTertiarySubdivision);
                var neighborhood = IncludeNeighborhood
                    ? FirstNonEmpty(address.Neighbourhood, address.MunicipalitySubdivision)
                    : String.Empty;

                yield return new AzureMapsAddress(
                    formattedAddress,
                    new Location(result.Position.Lat, result.Position.Lon),
                    BuildStreetLine(address.StreetNumber, address.StreetName),
                    FirstNonEmpty(address.CountrySubdivisionName, address.CountrySubdivision),
                    address.CountrySecondarySubdivision,
                    address.Country,
                    locality,
                    neighborhood,
                    address.PostalCode,
                    EvaluateEntityType(result),
                    EvaluateConfidence(result));
            }
            yield break;
        }

        if (response.Addresses is null)
            yield break;

        foreach (var reverseResult in response.Addresses.Where(result => result?.Address is not null && !String.IsNullOrWhiteSpace(result.Position)))
        {
            var reverseAddress = CreateReverseAddress(reverseResult);
            if (reverseAddress is not null)
                yield return reverseAddress;
        }
    }

    private AzureMapsAddress? CreateReverseAddress(AzureReverseResult? reverseResult)
    {
        if (reverseResult?.Address is null || !TryParsePosition(reverseResult.Position!, out var lat, out var lon))
            return null;

        var address = reverseResult.Address;
        var formattedAddress = FirstNonEmpty(address.FreeformAddress, address.StreetNameAndNumber, BuildStreetLine(address.StreetNumber, address.StreetName), FirstNonEmpty(address.LocalName, address.Municipality, address.CountryTertiarySubdivision), address.Country);
        if (String.IsNullOrWhiteSpace(formattedAddress))
            return null;

        var locality = FirstNonEmpty(address.LocalName, address.Municipality, address.CountryTertiarySubdivision);
        var neighborhood = IncludeNeighborhood
            ? FirstNonEmpty(address.Neighbourhood, address.MunicipalitySubdivision)
            : String.Empty;

        return new AzureMapsAddress(
            formattedAddress,
            new Location(lat, lon),
            BuildStreetLine(address.StreetNumber, address.StreetName),
            FirstNonEmpty(address.CountrySubdivisionName, address.CountrySubdivision),
            address.CountrySecondarySubdivision,
            address.Country,
            locality,
            neighborhood,
            address.PostalCode,
            EntityType.Address,
            ConfidenceLevel.High);
    }

    private static bool TryParsePosition(string position, out double latitude, out double longitude)
    {
        latitude = 0;
        longitude = 0;
        var parts = position.Split(',');
        return parts.Length == 2
            && double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out latitude)
            && double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out longitude);
    }

    private static string BuildStreetLine(string? streetNumber, string? streetName)
    {
        var parts = new[] { streetNumber, streetName }
            .Where(part => !String.IsNullOrWhiteSpace(part))
            .ToArray();

        return parts.Length == 0 ? String.Empty : String.Join(" ", parts);
    }

    private static string FirstNonEmpty(params string?[] values)
    {
        return values.FirstOrDefault(value => !String.IsNullOrWhiteSpace(value)) ?? String.Empty;
    }

    private static EntityType EvaluateEntityType(AzureSearchResult result)
    {
        var entityType = result.EntityType?.Trim();
        if (!String.IsNullOrWhiteSpace(entityType))
        {
            switch (entityType)
            {
                case "Country":
                    return EntityType.CountryRegion;
                case "CountrySubdivision":
                    return EntityType.AdminDivision1;
                case "CountrySecondarySubdivision":
                    return EntityType.AdminDivision2;
                case "CountryTertiarySubdivision":
                case "Municipality":
                case "MunicipalitySubdivision":
                case "Neighbourhood":
                    return EntityType.PopulatedPlace;
                case "PostalCodeArea":
                    return EntityType.Postcode1;
            }
        }

        switch (result.Type?.Trim())
        {
            case "POI":
                return EntityType.PointOfInterest;
            case "Point Address":
            case "Address Range":
            case "Cross Street":
                return EntityType.Address;
            case "Street":
                return EntityType.Road;
            case "Geography":
                return EntityType.PopulatedPlace;
            default:
                return EntityType.Address;
        }
    }

    private static ConfidenceLevel EvaluateConfidence(AzureSearchResult result)
    {
        switch (result.MatchType?.Trim())
        {
            case "AddressPoint":
                return ConfidenceLevel.High;
            case "HouseNumberRange":
                return ConfidenceLevel.Medium;
            case "Street":
                return ConfidenceLevel.Low;
        }

        switch (result.Type?.Trim())
        {
            case "Point Address":
            case "POI":
                return ConfidenceLevel.High;
            case "Address Range":
                return ConfidenceLevel.Medium;
            case "Street":
            case "Geography":
                return ConfidenceLevel.Low;
            default:
                return ConfidenceLevel.Unknown;
        }
    }

    private sealed class AzureSearchResponse
    {
        [JsonPropertyName("results")]
        public AzureSearchResult[] Results { get; set; } = Array.Empty<AzureSearchResult>();

        [JsonPropertyName("addresses")]
        public AzureReverseResult[] Addresses { get; set; } = Array.Empty<AzureReverseResult>();
    }

    private sealed class AzureReverseResult
    {
        [JsonPropertyName("address")]
        public AzureAddressPayload? Address { get; set; }

        [JsonPropertyName("position")]
        public string? Position { get; set; }
    }

    private sealed class AzureSearchResult
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("entityType")]
        public string? EntityType { get; set; }

        [JsonPropertyName("matchType")]
        public string? MatchType { get; set; }

        [JsonPropertyName("address")]
        public AzureAddressPayload? Address { get; set; }

        [JsonPropertyName("position")]
        public AzurePosition? Position { get; set; }

        [JsonPropertyName("poi")]
        public AzurePointOfInterest? Poi { get; set; }
    }

    private sealed class AzureAddressPayload
    {
        [JsonPropertyName("freeformAddress")]
        public string? FreeformAddress { get; set; }

        [JsonPropertyName("streetNumber")]
        public string? StreetNumber { get; set; }

        [JsonPropertyName("streetName")]
        public string? StreetName { get; set; }

        [JsonPropertyName("streetNameAndNumber")]
        public string? StreetNameAndNumber { get; set; }

        [JsonPropertyName("municipality")]
        public string? Municipality { get; set; }

        [JsonPropertyName("municipalitySubdivision")]
        public string? MunicipalitySubdivision { get; set; }

        [JsonPropertyName("neighbourhood")]
        public string? Neighbourhood { get; set; }

        [JsonPropertyName("localName")]
        public string? LocalName { get; set; }

        [JsonPropertyName("countrySubdivision")]
        public string? CountrySubdivision { get; set; }

        [JsonPropertyName("countrySubdivisionName")]
        public string? CountrySubdivisionName { get; set; }

        [JsonPropertyName("countrySecondarySubdivision")]
        public string? CountrySecondarySubdivision { get; set; }

        [JsonPropertyName("countryTertiarySubdivision")]
        public string? CountryTertiarySubdivision { get; set; }

        [JsonPropertyName("postalCode")]
        public string? PostalCode { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }
    }

    private sealed class AzurePosition
    {
        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lon")]
        public double Lon { get; set; }
    }

    private sealed class AzurePointOfInterest
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}
