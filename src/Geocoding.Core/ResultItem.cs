namespace Geocoding;

/// <summary>
/// Represents a single batch request item and its response values.
/// </summary>
public class ResultItem
{
    private Address _input;
    /// <summary>
    /// Original input for this response
    /// </summary>
    public Address Request
    {
        get { return _input; }
        set
        {
            if (value == null)
                throw new ArgumentNullException("Input");

            _input = value;
        }
    }

    private IEnumerable<Address> _output;
    /// <summary>
    /// Output for the given input
    /// </summary>
    public IEnumerable<Address> Response
    {
        get { return _output; }
        set
        {
            if (value == null)
                throw new ArgumentNullException("Response");

            _output = value;
        }
    }

    /// <summary>
    /// Initializes a new result item.
    /// </summary>
    /// <param name="request">The request address.</param>
    /// <param name="response">The response addresses.</param>
    public ResultItem(Address request, IEnumerable<Address> response)
    {
        Request = request;
        Response = response;
    }
}
