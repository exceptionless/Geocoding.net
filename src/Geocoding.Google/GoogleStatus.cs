namespace Geocoding.Google;

/// <summary>
/// Represents the status returned by the Google geocoding service.
/// </summary>
public enum GoogleStatus
{
    /// <summary>The Error value.</summary>
    Error,
    /// <summary>The Ok value.</summary>
    Ok,
    /// <summary>The ZeroResults value.</summary>
    ZeroResults,
    /// <summary>The OverQueryLimit value.</summary>
    OverQueryLimit,
    /// <summary>The RequestDenied value.</summary>
    RequestDenied,
    /// <summary>The InvalidRequest value.</summary>
    InvalidRequest
}
