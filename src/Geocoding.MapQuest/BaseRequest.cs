using System.Text;
using System.Text.Json.Serialization;

namespace Geocoding.MapQuest;

/// <summary>
/// Geo-code request object.
/// See https://developer.mapquest.com/documentation/api/geocoding/.
/// </summary>
public abstract class BaseRequest
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseRequest"/> class.
    /// </summary>
    /// <param name="key">The MapQuest application key.</param>
    protected BaseRequest(string key) //output only, no need for default ctor
    {
        Key = key;
    }

    [JsonIgnore] private string _key = null!;
    /// <summary>
    /// A required unique key to authorize use of the routing service.
    /// See https://developer.mapquest.com/documentation/api/geocoding/.
    /// </summary>
    [JsonIgnore]
    public virtual string Key
    {
        get { return _key; }
        set
        {
            if (String.IsNullOrWhiteSpace(value))
                throw new ArgumentException("An application key is required for MapQuest");

            _key = value;
        }
    }

    /// <summary>
    /// Defaults to json
    /// </summary>
    [JsonIgnore]
    public virtual DataFormat InputFormat { get; private set; }

    /// <summary>
    /// Defaults to json
    /// </summary>
    [JsonIgnore]
    public virtual DataFormat OutputFormat { get; private set; }

    [JsonIgnore] private RequestOptions _op = new RequestOptions();
    /// <summary>
    /// Optional settings
    /// </summary>
    [JsonPropertyName("options")]
    public virtual RequestOptions Options
    {
        get { return _op; }
        protected set
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            _op = value;
        }
    }

    /// <summary>
    /// if true, use Open Street Map, else use commercial map
    /// </summary>
    public virtual bool UseOSM { get; set; }

    /// <summary>
    /// Uses the commercial MapQuest geocoding API.
    /// </summary>
    protected virtual string BaseRequestPath
    {
        get
        {
            if (UseOSM)
                throw new NotSupportedException("MapQuest OpenStreetMap geocoding is no longer supported. Use the commercial MapQuest API instead.");

            return @"https://www.mapquestapi.com/geocoding/v1/";
        }
    }

    /// <summary>
    /// The full path for the request
    /// </summary>
    [JsonIgnore]
    public virtual Uri RequestUri
    {
        get
        {
            var sb = new StringBuilder(BaseRequestPath);
            sb.Append(RequestAction);
            sb.Append("?");
            //no need to escape this key, it is already escaped by MapQuest at generation
            sb.AppendFormat("key={0}&", Key);

            if (InputFormat != DataFormat.json)
                sb.AppendFormat("inFormat={0}&", InputFormat);

            if (OutputFormat != DataFormat.json)
                sb.AppendFormat("outFormat={0}&", OutputFormat);

            sb.Length--;
            return new Uri(sb.ToString());
        }
    }

    /// <summary>
    /// Gets the request action path segment.
    /// </summary>
    [JsonIgnore]
    public abstract string RequestAction { get; }

    [JsonIgnore] private string _verb = "POST";
    /// <summary>
    /// Default request verb is POST for security and large batch payloads
    /// </summary>
    [JsonIgnore]
    public virtual string RequestVerb
    {
        get { return _verb; }
        protected set { _verb = String.IsNullOrWhiteSpace(value) ? "POST" : value.Trim().ToUpper(); }
    }

    /// <summary>
    /// Request body if request verb is applicable (POST, PUT, etc)
    /// </summary>
    [JsonIgnore]
    public virtual string RequestBody
    {
        get
        {
            return this.ToJSON();
        }
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return this.RequestBody;
    }
}
