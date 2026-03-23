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
        if (TryGetCachedSkipReason(apiKey, out string? skipReason))
        {
            if (!String.IsNullOrWhiteSpace(skipReason))
                Assert.Skip(skipReason);

            return;
        }

        string? validatedSkipReason = ValidateCore(apiKey);

        lock (_sync)
        {
            if (!_validated || !String.Equals(_validatedApiKey, apiKey, StringComparison.Ordinal))
            {
                _validatedApiKey = apiKey;
                _skipReason = validatedSkipReason;
                _validated = true;
            }

            skipReason = _skipReason;
        }

        if (!String.IsNullOrWhiteSpace(skipReason))
            Assert.Skip(skipReason);
    }

    private static bool TryGetCachedSkipReason(string apiKey, out string? skipReason)
    {
        lock (_sync)
        {
            if (_validated && String.Equals(_validatedApiKey, apiKey, StringComparison.Ordinal))
            {
                skipReason = _skipReason;
                return true;
            }
        }

        skipReason = null;
        return false;
    }

    private static string? ValidateCore(string apiKey)
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
            var geocoder = new GoogleGeocoder(apiKey);
            _ = geocoder.GeocodeAsync("1600 pennsylvania ave nw, washington dc", cts.Token)
                .GetAwaiter()
                .GetResult()
                .FirstOrDefault();

            return null;
        }
        catch (GoogleGeocodingException ex) when (ex.Status is GoogleStatus.RequestDenied or GoogleStatus.OverDailyLimit or GoogleStatus.OverQueryLimit)
        {
            return BuildSkipReason(ex);
        }
        catch (OperationCanceledException)
        {
            return "Google integration test guard timed out while validating API key availability.";
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
