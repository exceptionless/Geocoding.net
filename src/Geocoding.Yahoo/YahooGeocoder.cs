using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Xml.XPath;

namespace Geocoding.Yahoo;

/// <summary>
/// Provides geocoding and reverse geocoding through the Yahoo geocoding API.
/// </summary>
/// <remarks>
/// http://developer.yahoo.com/geo/placefinder/
/// </remarks>
[Obsolete("Yahoo PlaceFinder/BOSS geocoding has been discontinued. This type is retained for source compatibility only and will be removed in a future major version.")]
public class YahooGeocoder : IGeocoder
{
    /// <summary>
    /// The single-line Yahoo geocoding service URL format.
    /// </summary>
    public const string ServiceUrl = "http://yboss.yahooapis.com/geo/placefinder?q={0}";
    /// <summary>
    /// The multi-part Yahoo geocoding service URL format.
    /// </summary>
    public const string ServiceUrlNormal = "http://yboss.yahooapis.com/geo/placefinder?street={0}&city={1}&state={2}&postal={3}&country={4}";
    /// <summary>
    /// The Yahoo reverse geocoding service URL format.
    /// </summary>
    public const string ServiceUrlReverse = "http://yboss.yahooapis.com/geo/placefinder?q={0}&gflags=R";

    private readonly string _consumerKey, _consumerSecret;

    /// <summary>
    /// Gets the Yahoo consumer key.
    /// </summary>
    public string ConsumerKey
    {
        get { return _consumerKey; }
    }

    /// <summary>
    /// Gets the Yahoo consumer secret.
    /// </summary>
    public string ConsumerSecret
    {
        get { return _consumerSecret; }
    }

    /// <summary>
    /// Gets or sets the proxy used for Yahoo requests.
    /// </summary>
    public IWebProxy? Proxy { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="YahooGeocoder"/> class.
    /// </summary>
    /// <param name="consumerKey">The Yahoo consumer key.</param>
    /// <param name="consumerSecret">The Yahoo consumer secret.</param>
    public YahooGeocoder(string consumerKey, string consumerSecret)
    {
        if (String.IsNullOrEmpty(consumerKey))
            throw new ArgumentNullException(nameof(consumerKey));

        if (String.IsNullOrEmpty(consumerSecret))
            throw new ArgumentNullException(nameof(consumerSecret));

        _consumerKey = consumerKey;
        _consumerSecret = consumerSecret;
    }

    /// <inheritdoc />
    public Task<IEnumerable<YahooAddress>> GeocodeAsync(string address, CancellationToken cancellationToken = default(CancellationToken))
    {
        if (String.IsNullOrEmpty(address))
            throw new ArgumentNullException(nameof(address));

        string url = String.Format(ServiceUrl, WebUtility.UrlEncode(address));

        HttpRequestMessage request = BuildRequest(url);
        return ProcessRequest(request, cancellationToken);
    }

    /// <inheritdoc />
    public Task<IEnumerable<YahooAddress>> GeocodeAsync(string street, string city, string state, string postalCode, string country, CancellationToken cancellationToken = default(CancellationToken))
    {
        string url = String.Format(ServiceUrlNormal, WebUtility.UrlEncode(street), WebUtility.UrlEncode(city), WebUtility.UrlEncode(state), WebUtility.UrlEncode(postalCode), WebUtility.UrlEncode(country));

        HttpRequestMessage request = BuildRequest(url);
        return ProcessRequest(request, cancellationToken);
    }

    /// <inheritdoc />
    public Task<IEnumerable<YahooAddress>> ReverseGeocodeAsync(Location location, CancellationToken cancellationToken = default(CancellationToken))
    {
        if (location is null)
            throw new ArgumentNullException(nameof(location));

        return ReverseGeocodeAsync(location.Latitude, location.Longitude, cancellationToken);
    }

    /// <inheritdoc />
    public Task<IEnumerable<YahooAddress>> ReverseGeocodeAsync(double latitude, double longitude, CancellationToken cancellationToken = default(CancellationToken))
    {
        string url = String.Format(ServiceUrlReverse, String.Format(CultureInfo.InvariantCulture, "{0} {1}", latitude, longitude));

        HttpRequestMessage request = BuildRequest(url);
        return ProcessRequest(request, cancellationToken);
    }

    private async Task<IEnumerable<YahooAddress>> ProcessRequest(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            using var requestToDispose = request;
            using var client = BuildClient();
            using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var preview = await BuildResponsePreviewAsync(response.Content).ConfigureAwait(false);
                var message = $"Yahoo request failed ({(int)response.StatusCode} {response.ReasonPhrase}).{preview}";

                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException ex)
                {
                    throw new YahooGeocodingException(message, ex);
                }

                throw new YahooGeocodingException(message, new HttpRequestException(message));
            }

            using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                cancellationToken.ThrowIfCancellationRequested();
                return ProcessResponse(stream);
            }
        }
        catch (YahooGeocodingException)
        {
            //let these pass through
            throw;
        }
        catch (Exception ex)
        {
            //wrap in yahoo exception
            throw new YahooGeocodingException(ex);
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

    /// <summary>
    /// Builds the HTTP client used for Yahoo requests.
    /// </summary>
    /// <returns>The configured HTTP client.</returns>
    protected virtual HttpClient BuildClient()
    {
        if (Proxy is null)
            return new HttpClient();

        return new HttpClient(new HttpClientHandler { Proxy = Proxy });
    }

    private HttpRequestMessage BuildRequest(string url)
    {
        url = GenerateOAuthSignature(new Uri(url));
        return new HttpRequestMessage(HttpMethod.Get, url);
    }

    private static async Task<string> BuildResponsePreviewAsync(HttpContent content)
    {
        using var stream = await content.ReadAsStreamAsync().ConfigureAwait(false);
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: false);

        char[] buffer = new char[256];
        int read = await reader.ReadBlockAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
        if (read == 0)
            return String.Empty;

        var preview = new string(buffer, 0, read).Trim();
        if (String.IsNullOrWhiteSpace(preview))
            return String.Empty;

        return " Response preview: " + preview + (reader.EndOfStream ? String.Empty : "...");
    }

    private string GenerateOAuthSignature(Uri uri)
    {
        string url, param;

        var oAuth = new OAuthBase();
        var nonce = oAuth.GenerateNonce();
        var timeStamp = oAuth.GenerateTimeStamp();

        var signature = oAuth.GenerateSignature(
            uri,
            _consumerKey,
            _consumerSecret,
            String.Empty,
            String.Empty,
            "GET",
            timeStamp,
            nonce,
            OAuthBase.SignatureTypes.HMACSHA1,
            out url,
            out param
        );

        return $"{url}?{param}&oauth_signature={signature}";
    }

    private IEnumerable<YahooAddress> ProcessResponse(Stream stream)
    {
        XPathDocument xmlDoc = LoadXmlResponse(stream);
        XPathNavigator nav = xmlDoc.CreateNavigator();

        YahooError error = EvaluateError(Convert.ToInt32(nav.Evaluate("number(/ResultSet/Error)")));

        if (error != YahooError.NoError)
            throw new YahooGeocodingException(error);

        return ParseAddresses(nav.Select("/ResultSet/Result")).ToArray();
    }

    private XPathDocument LoadXmlResponse(Stream stream)
    {
        XPathDocument doc = new XPathDocument(stream);
        return doc;
    }

    private IEnumerable<YahooAddress> ParseAddresses(XPathNodeIterator nodes)
    {
        while (nodes.MoveNext())
        {
            XPathNavigator nav = nodes.Current!;

            int quality = Convert.ToInt32(nav.Evaluate("number(quality)"));
            string formattedAddress = ParseFormattedAddress(nav);

            double latitude = (double)nav.Evaluate("number(latitude)");
            double longitude = (double)nav.Evaluate("number(longitude)");
            Location coordinates = new Location(latitude, longitude);

            string name = (string)nav.Evaluate("string(name)");
            string house = (string)nav.Evaluate("string(house)");
            string street = (string)nav.Evaluate("string(street)");
            string unit = (string)nav.Evaluate("string(unit)");
            string unitType = (string)nav.Evaluate("string(unittype)");
            string neighborhood = (string)nav.Evaluate("string(neighborhood)");
            string city = (string)nav.Evaluate("string(city)");
            string county = (string)nav.Evaluate("string(county)");
            string countyCode = (string)nav.Evaluate("string(countycode)");
            string state = (string)nav.Evaluate("string(state)");
            string stateCode = (string)nav.Evaluate("string(statecode)");
            string postalCode = (string)nav.Evaluate("string(postal)");
            string country = (string)nav.Evaluate("string(country)");
            string countryCode = (string)nav.Evaluate("string(countrycode)");

            yield return new YahooAddress(
                formattedAddress,
                coordinates,
                name,
                house,
                street,
                unit,
                unitType,
                neighborhood,
                city,
                county,
                countyCode,
                state,
                stateCode,
                postalCode,
                country,
                countryCode,
                quality
            );
        }
    }

    private string ParseFormattedAddress(XPathNavigator nav)
    {
        string[] lines = new string[4];
        lines[0] = (string)nav.Evaluate("string(line1)");
        lines[1] = (string)nav.Evaluate("string(line2)");
        lines[2] = (string)nav.Evaluate("string(line3)");
        lines[3] = (string)nav.Evaluate("string(line4)");

        lines = lines.Select(s => (s ?? "").Trim()).Where(s => !String.IsNullOrEmpty(s)).ToArray();
        return String.Join(", ", lines);
    }

    private YahooError EvaluateError(int errorCode)
    {
        if (errorCode >= 1000)
            return YahooError.UnknownError;

        return (YahooError)errorCode;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Yahoo Geocoder: {_consumerKey}, {_consumerSecret}";
    }
}
