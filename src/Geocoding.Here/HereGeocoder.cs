using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Geocoding.Here;

/// <summary>
/// Provides geocoding and reverse geocoding through the HERE geocoding API.
/// </summary>
/// <remarks>
/// https://developer.here.com/documentation/geocoder/topics/request-constructing.html
/// </remarks>
public class HereGeocoder : IGeocoder
{
    private const string GeocodingQuery = "https://geocoder.api.here.com/6.2/geocode.json?app_id={0}&app_code={1}&{2}";
    private const string ReverseGeocodingQuery = "https://reverse.geocoder.api.here.com/6.2/reversegeocode.json?app_id={0}&app_code={1}&mode=retrieveAddresses&{2}";
    private const string Searchtext = "searchtext={0}";
    private const string Prox = "prox={0}";
    private const string Street = "street={0}";
    private const string City = "city={0}";
    private const string State = "state={0}";
    private const string PostalCode = "postalcode={0}";
    private const string Country = "country={0}";

    private readonly string _appId;
    private readonly string _appCode;

    /// <summary>
    /// Gets or sets the proxy used for HERE requests.
    /// </summary>
    public IWebProxy Proxy { get; set; }
    /// <summary>
    /// Gets or sets the user location bias for requests.
    /// </summary>
    public Location UserLocation { get; set; }
    /// <summary>
    /// Gets or sets the map view bias for requests.
    /// </summary>
    public Bounds UserMapView { get; set; }
    /// <summary>
    /// Gets or sets the maximum number of results to request.
    /// </summary>
    public int? MaxResults { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HereGeocoder"/> class.
    /// </summary>
    /// <param name="appId">The HERE application identifier.</param>
    /// <param name="appCode">The HERE application code.</param>
    public HereGeocoder(string appId, string appCode)
    {
        if (string.IsNullOrWhiteSpace(appId))
            throw new ArgumentException("appId can not be null or empty");

        if (string.IsNullOrWhiteSpace(appCode))
            throw new ArgumentException("appCode can not be null or empty");

        _appId = appId;
        _appCode = appCode;
    }

    private string GetQueryUrl(string address)
    {
        var parameters = new StringBuilder();
        var first = AppendParameter(parameters, address, Searchtext, true);
        AppendGlobalParameters(parameters, first);

        return string.Format(GeocodingQuery, _appId, _appCode, parameters.ToString());
    }

    private string GetQueryUrl(string street, string city, string state, string postalCode, string country)
    {
        var parameters = new StringBuilder();
        var first = AppendParameter(parameters, street, Street, true);
        first = AppendParameter(parameters, city, City, first);
        first = AppendParameter(parameters, state, State, first);
        first = AppendParameter(parameters, postalCode, PostalCode, first);
        first = AppendParameter(parameters, country, Country, first);
        AppendGlobalParameters(parameters, first);

        return string.Format(GeocodingQuery, _appId, _appCode, parameters.ToString());
    }

    private string GetQueryUrl(double latitude, double longitude)
    {
        var parameters = new StringBuilder();
        var first = AppendParameter(parameters, string.Format(CultureInfo.InvariantCulture, "{0},{1}", latitude, longitude), Prox, true);
        AppendGlobalParameters(parameters, first);

        return string.Format(ReverseGeocodingQuery, _appId, _appCode, parameters.ToString());
    }

    private IEnumerable<KeyValuePair<string, string>> GetGlobalParameters()
    {
        if (UserLocation != null)
            yield return new KeyValuePair<string, string>("prox", UserLocation.ToString());

        if (UserMapView != null)
            yield return new KeyValuePair<string, string>("mapview", string.Concat(UserMapView.SouthWest.ToString(), ",", UserMapView.NorthEast.ToString()));

        if (MaxResults != null && MaxResults.Value > 0)
            yield return new KeyValuePair<string, string>("maxresults", MaxResults.Value.ToString(CultureInfo.InvariantCulture));
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

            builder.Append(UrlEncode(pair.Key));
            builder.Append("=");
            builder.Append(UrlEncode(pair.Value));
        }
        return builder.ToString();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<HereAddress>> GeocodeAsync(string address, CancellationToken cancellationToken = default(CancellationToken))
    {
        try
        {
            var url = GetQueryUrl(address);
            var response = await GetResponse(url, cancellationToken).ConfigureAwait(false);
            return ParseResponse(response);
        }
        catch (Exception ex)
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
        catch (Exception ex)
        {
            throw new HereGeocodingException(ex);
        }
    }

    /// <inheritdoc />
    public Task<IEnumerable<HereAddress>> ReverseGeocodeAsync(Location location, CancellationToken cancellationToken = default(CancellationToken))
    {
        if (location == null)
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
        catch (Exception ex)
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

    private bool AppendParameter(StringBuilder sb, string parameter, string format, bool first)
    {
        if (!string.IsNullOrEmpty(parameter))
        {
            if (!first)
            {
                sb.Append('&');
            }
            sb.Append(string.Format(format, UrlEncode(parameter)));
            return false;
        }
        return first;
    }

    private IEnumerable<HereAddress> ParseResponse(Json.Response response)
    {
        foreach (var view in response.View)
        {
            foreach (var result in view.Result)
            {
                var location = result.Location;
                yield return new HereAddress(
                    location.Address.Label,
                    new Location(location.DisplayPosition.Latitude, location.DisplayPosition.Longitude),
                    location.Address.Street,
                    location.Address.HouseNumber,
                    location.Address.City,
                    location.Address.State,
                    location.Address.PostalCode,
                    location.Address.Country,
                    (HereLocationType)Enum.Parse(typeof(HereLocationType), location.LocationType, true));
            }
        }
    }

    private HttpRequestMessage CreateRequest(string url)
    {
        return new HttpRequestMessage(HttpMethod.Get, url);
    }

    private HttpClient BuildClient()
    {
        if (Proxy == null)
            return new HttpClient();

        var handler = new HttpClientHandler { Proxy = Proxy };
        return new HttpClient(handler);
    }

    private async Task<Json.Response> GetResponse(string queryUrl, CancellationToken cancellationToken)
    {
        using (var client = BuildClient())
        {
            var response = await client.SendAsync(CreateRequest(queryUrl), cancellationToken).ConfigureAwait(false);
            using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                var jsonSerializer = new DataContractJsonSerializer(typeof(Json.ServerResponse));
                var serverResponse = (Json.ServerResponse)jsonSerializer.ReadObject(stream);

                if (serverResponse.ErrorType != null)
                {
                    throw new HereGeocodingException(serverResponse.Details, serverResponse.ErrorType, serverResponse.ErrorType);
                }

                return serverResponse.Response;
            }
        }
    }

    private string UrlEncode(string toEncode)
    {
        if (string.IsNullOrEmpty(toEncode))
            return string.Empty;

        return WebUtility.UrlEncode(toEncode);
    }
}
