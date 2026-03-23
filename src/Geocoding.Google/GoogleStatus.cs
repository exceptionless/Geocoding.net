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
    InvalidRequest,
    /// <summary>The OverDailyLimit value (billing or API key issue).</summary>
    OverDailyLimit,
    /// <summary>The UnknownError value (server-side error).</summary>
    UnknownError
}
