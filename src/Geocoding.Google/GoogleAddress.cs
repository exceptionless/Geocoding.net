namespace Geocoding.Google;

/// <summary>
/// Represents an address returned by the Google geocoding service.
/// </summary>
public class GoogleAddress : Address
{
    private readonly GoogleAddressType _type;
    private readonly GoogleLocationType _locationType;
    private readonly GoogleAddressComponent[] _components;
    private readonly bool _isPartialMatch;
    private readonly GoogleViewport _viewport;
    private readonly Bounds _bounds;
    private readonly string _placeId;

    /// <summary>
    /// Gets the primary Google address type for the result.
    /// </summary>
    public GoogleAddressType Type
    {
        get { return _type; }
    }

    /// <summary>
    /// Gets the Google location type for the result.
    /// </summary>
    public GoogleLocationType LocationType
    {
        get { return _locationType; }
    }

    /// <summary>
    /// Gets the Google address components for the result.
    /// </summary>
    public GoogleAddressComponent[] Components
    {
        get { return _components; }
    }

    /// <summary>
    /// Gets a value indicating whether the result is a partial match.
    /// </summary>
    public bool IsPartialMatch
    {
        get { return _isPartialMatch; }
    }

    /// <summary>
    /// Gets the viewport returned by Google.
    /// </summary>
    public GoogleViewport Viewport
    {
        get { return _viewport; }
    }

    /// <summary>
    /// Gets the bounds returned by Google.
    /// </summary>
    public Bounds Bounds
    {
        get { return _bounds; }
    }

    /// <summary>
    /// Gets the Google place identifier.
    /// </summary>
    public string PlaceId
    {
        get { return _placeId; }
    }

    /// <summary>
    /// Gets the first address component matching the specified type.
    /// </summary>
    /// <param name="type">The component type to locate.</param>
    /// <returns>The matching component, or <c>null</c> if no component matches.</returns>
    public GoogleAddressComponent this[GoogleAddressType type]
    {
        get { return Components.FirstOrDefault(c => c.Types.Contains(type)); }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GoogleAddress"/> class.
    /// </summary>
    /// <param name="type">The primary address type.</param>
    /// <param name="formattedAddress">The formatted address returned by Google.</param>
    /// <param name="components">The parsed address components.</param>
    /// <param name="coordinates">The coordinates returned by Google.</param>
    /// <param name="viewport">The viewport returned by Google.</param>
    /// <param name="bounds">The bounds returned by Google.</param>
    /// <param name="isPartialMatch">Whether the result is a partial match.</param>
    /// <param name="locationType">The location type returned by Google.</param>
    /// <param name="placeId">The Google place identifier.</param>
    public GoogleAddress(GoogleAddressType type, string formattedAddress, GoogleAddressComponent[] components,
        Location coordinates, GoogleViewport viewport, Bounds bounds, bool isPartialMatch, GoogleLocationType locationType, string placeId)
        : base(formattedAddress, coordinates, "Google")
    {
        if (components == null)
            throw new ArgumentNullException("components");

        _type = type;
        _components = components;
        _isPartialMatch = isPartialMatch;
        _viewport = viewport;
        _bounds = bounds;
        _locationType = locationType;
        _placeId = placeId;
    }
}
