using System.Net;
using System.Net.Http;
using System.Text;
using Geocoding.Collections;
using Geocoding.Serialization;

namespace Geocoding.MapQuest;

/// <summary>
/// Provides geocoding and reverse geocoding through the MapQuest API.
/// </summary>
/// <remarks>
/// See https://developer.mapquest.com/documentation/api/geocoding/.
/// </remarks>
public class MapQuestGeocoder : IGeocoder, IBatchGeocoder
{
    private readonly string _key;

    private volatile bool _useOsm;
    /// <summary>
    /// Enables the legacy OpenStreetMap-backed MapQuest endpoint.
    /// </summary>
    public virtual bool UseOSM
    {
        get { return _useOsm; }
        set
        {
            if (value)
                throw new NotSupportedException("MapQuest OpenStreetMap geocoding is no longer supported. Use the commercial MapQuest API instead.");

            _useOsm = false;
        }
    }

    /// <summary>
    /// Gets or sets the proxy used for MapQuest requests.
    /// </summary>
    public IWebProxy? Proxy { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MapQuestGeocoder"/> class.
    /// </summary>
    /// <param name="key">The MapQuest application key.</param>
    public MapQuestGeocoder(string key)
    {
        if (String.IsNullOrWhiteSpace(key))
            throw new ArgumentException("key can not be null or blank.", nameof(key));

        _key = key;
    }

    private IEnumerable<Address> HandleSingleResponse(MapQuestResponse res)
    {
        return res is not null && !CollectionExtensions.IsNullOrEmpty(res.Results)
            ? HandleSingleResponse(from r in res.Results.OfType<MapQuestResult>()
                                   from l in r.Locations?.OfType<MapQuestLocation>() ?? Enumerable.Empty<MapQuestLocation>()
                                   select l)
            : Array.Empty<Address>();
    }

    private IEnumerable<Address> HandleSingleResponse(IEnumerable<MapQuestLocation> locs)
    {
        if (locs is null)
            return Array.Empty<Address>();
        else
        {
            return from l in locs.OfType<MapQuestLocation>()
                   where l.Quality < Quality.COUNTRY
                   let q = (int)l.Quality
                   let c = String.IsNullOrWhiteSpace(l.Confidence) ? "ZZZZZZ" : l.Confidence
                   orderby q ascending, c ascending
                   select l;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Address>> GeocodeAsync(string address, CancellationToken cancellationToken = default(CancellationToken))
    {
        if (String.IsNullOrWhiteSpace(address))
            throw new ArgumentException("address can not be null or empty.", nameof(address));

        var f = new GeocodeRequest(_key, address) { UseOSM = UseOSM };
        MapQuestResponse res = await Execute(f, cancellationToken).ConfigureAwait(false);
        return HandleSingleResponse(res);
    }

    /// <inheritdoc />
    public Task<IEnumerable<Address>> GeocodeAsync(string street, string city, string state, string postalCode, string country, CancellationToken cancellationToken = default(CancellationToken))
    {
        var sb = new StringBuilder();
        if (!String.IsNullOrWhiteSpace(street))
            sb.AppendFormat("{0}, ", street);
        if (!String.IsNullOrWhiteSpace(city))
            sb.AppendFormat("{0}, ", city);
        if (!String.IsNullOrWhiteSpace(state))
            sb.AppendFormat("{0} ", state);
        if (!String.IsNullOrWhiteSpace(postalCode))
            sb.AppendFormat("{0} ", postalCode);
        if (!String.IsNullOrWhiteSpace(country))
            sb.AppendFormat("{0} ", country);

        if (sb.Length > 1)
            sb.Length--;

        string s = sb.ToString().Trim();
        if (String.IsNullOrWhiteSpace(s))
            throw new ArgumentException("Concatenated input values can not be null or blank");

        if (s.Last() == ',')
            s = s.Remove(s.Length - 1);

        return GeocodeAsync(s, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Address>> ReverseGeocodeAsync(Location location, CancellationToken cancellationToken = default(CancellationToken))
    {
        if (location is null)
            throw new ArgumentNullException(nameof(location));

        var f = new ReverseGeocodeRequest(_key, location) { UseOSM = UseOSM };
        MapQuestResponse res = await Execute(f, cancellationToken).ConfigureAwait(false);
        return HandleSingleResponse(res);
    }

    /// <inheritdoc />
    public Task<IEnumerable<Address>> ReverseGeocodeAsync(double latitude, double longitude, CancellationToken cancellationToken = default(CancellationToken))
    {
        return ReverseGeocodeAsync(new Location(latitude, longitude), cancellationToken);
    }

    /// <summary>
    /// Executes a raw MapQuest request and returns the deserialized response.
    /// </summary>
    /// <param name="f">The request to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deserialized MapQuest response.</returns>
    public async Task<MapQuestResponse> Execute(BaseRequest f, CancellationToken cancellationToken = default(CancellationToken))
    {
        using var client = BuildClient();
        using var request = CreateRequest(f);
        MapQuestResponse r = await Parse(client, request, cancellationToken).ConfigureAwait(false);
        if (r is not null && !CollectionExtensions.IsNullOrEmpty(r.Results))
        {
            foreach (MapQuestResult o in r.Results)
            {
                if (o is null)
                    continue;

                foreach (MapQuestLocation l in o.Locations ?? Array.Empty<MapQuestLocation>())
                {
                    if (!String.IsNullOrWhiteSpace(l.FormattedAddress) || o.ProvidedLocation is null)
                        continue;

                    if (string.Compare(o.ProvidedLocation.FormattedAddress, "unknown", true) != 0)
                        l.FormattedAddress = o.ProvidedLocation.FormattedAddress;
                    else
                        l.FormattedAddress = o.ProvidedLocation.ToString();
                }
            }
        }
        return r!;
    }

    /// <summary>
    /// Builds the HTTP client used for MapQuest requests.
    /// </summary>
    /// <returns>The configured HTTP client.</returns>
    protected virtual HttpClient BuildClient()
    {
        if (Proxy is null)
            return new HttpClient();

        return new HttpClient(new HttpClientHandler { Proxy = Proxy });
    }

    private HttpRequestMessage CreateRequest(BaseRequest f)
    {
        if (f is null)
            throw new ArgumentNullException(nameof(f));

        Uri requestUri;
        switch (f.RequestVerb)
        {
            case "GET":
            case "DELETE":
            case "HEAD":
            {
                var u = $"{f.RequestUri}json={WebUtility.UrlEncode(f.RequestBody)}&";
                requestUri = new Uri(u, UriKind.Absolute);
            }
            break;
            case "POST":
            case "PUT":
            default:
            {
                requestUri = f.RequestUri;
            }
            break;
        }

        var request = new HttpRequestMessage(new HttpMethod(f.RequestVerb), requestUri);
        if (!String.IsNullOrWhiteSpace(f.RequestBody)
            && !String.Equals(f.RequestVerb, "GET", StringComparison.OrdinalIgnoreCase)
            && !String.Equals(f.RequestVerb, "DELETE", StringComparison.OrdinalIgnoreCase)
            && !String.Equals(f.RequestVerb, "HEAD", StringComparison.OrdinalIgnoreCase))
        {
            request.Content = new StringContent(f.RequestBody, Encoding.UTF8, "application/" + f.InputFormat);
        }

        return request;
    }

    private async Task<MapQuestResponse> Parse(HttpClient client, HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string requestInfo = $"[{request.Method}] {request.RequestUri}";
        try
        {
            using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"{(int)response.StatusCode} {requestInfo} | {response.ReasonPhrase}{BuildResponsePreview(json)}");

            if (String.IsNullOrWhiteSpace(json))
                throw new Exception("Remote system response with blank: " + requestInfo);

            MapQuestResponse? o = JsonExtensions.FromJSON<MapQuestResponse>(json);
            if (o is null)
                throw new Exception("Unable to deserialize remote response: " + requestInfo);

            return o;
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"{requestInfo} | {ex.Message}", ex);
        }
    }

    private static string BuildResponsePreview(string? body)
    {
        if (String.IsNullOrWhiteSpace(body))
            return String.Empty;

        var preview = body!.Trim();
        if (preview.Length > 256)
            preview = preview.Substring(0, 256) + "...";

        return " | Response preview: " + preview;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ResultItem>> GeocodeAsync(IEnumerable<string> addresses, CancellationToken cancellationToken = default(CancellationToken))
    {
        if (addresses is null)
            throw new ArgumentNullException(nameof(addresses));

        string[] adr = (from a in addresses
                        where !String.IsNullOrWhiteSpace(a)
                        group a by a into ag
                        select ag.Key).ToArray();
        if (CollectionExtensions.IsNullOrEmpty(adr))
            throw new ArgumentException("Atleast one none blank item is required in addresses");

        var f = new BatchGeocodeRequest(_key, adr) { UseOSM = UseOSM };
        MapQuestResponse res = await Execute(f, cancellationToken).ConfigureAwait(false);
        return HandleBatchResponse(res);
    }

    private ICollection<ResultItem> HandleBatchResponse(MapQuestResponse res)
    {
        if (res is not null && !CollectionExtensions.IsNullOrEmpty(res.Results))
        {
            return (from r in res.Results.OfType<MapQuestResult>()
                    let locations = r.Locations?.OfType<MapQuestLocation>().ToArray() ?? Array.Empty<MapQuestLocation>()
                    where locations.Length > 0
                    let resp = HandleSingleResponse(locations)
                    select new ResultItem(r.ProvidedLocation!, resp)).ToArray();
        }
        else
            return new ResultItem[0];
    }

    /// <inheritdoc />
    public Task<IEnumerable<ResultItem>> ReverseGeocodeAsync(IEnumerable<Location> locations, CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotSupportedException("ReverseGeocodeAsync(...) is not available for MapQuestGeocoder.");
    }
}
