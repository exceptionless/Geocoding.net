using Geocoding.Google;
using Xunit;

namespace Geocoding.Tests;

internal static class GoogleTestGuard
{
    private static readonly object _sync = new();
    private static bool _validated;
    private static string? _validatedApiKey;
    private static string? _skipReason;

    public static void EnsureAvailable(string apiKey)
    {
        string? skipReason;

        lock (_sync)
        {
            if (_validated && String.Equals(_validatedApiKey, apiKey, StringComparison.Ordinal))
            {
                skipReason = _skipReason;
            }
            else
            {
                skipReason = ValidateCore(apiKey);
                _validatedApiKey = apiKey;
                _skipReason = skipReason;
                _validated = true;
            }
        }

        if (!String.IsNullOrWhiteSpace(skipReason))
            Assert.Skip(skipReason);
    }

    private static string? ValidateCore(string apiKey)
    {
        try
        {
            var geocoder = new GoogleGeocoder(apiKey);
            _ = geocoder.GeocodeAsync("1600 pennsylvania ave nw, washington dc", CancellationToken.None)
                .GetAwaiter()
                .GetResult()
                .FirstOrDefault();

            return null;
        }
        catch (GoogleGeocodingException ex) when (ex.Status is GoogleStatus.RequestDenied or GoogleStatus.OverDailyLimit or GoogleStatus.OverQueryLimit)
        {
            return BuildSkipReason(ex);
        }
    }

    private static string BuildSkipReason(GoogleGeocodingException ex)
    {
        string providerMessage = String.IsNullOrWhiteSpace(ex.ProviderMessage)
            ? "Google denied the request for the configured API key."
            : ex.ProviderMessage;

        return $"Google integration tests require a working Google Geocoding API key with billing/quota access. Status={ex.Status}. {providerMessage}";
    }
}
