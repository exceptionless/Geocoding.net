using Xunit;

namespace Geocoding.Tests;

public static class AddressAssertionExtensions
{
    public static void AssertWhiteHouse(this Address address)
    {
        String adr = address.FormattedAddress.ToLowerInvariant();
        Assert.True(
            adr.Contains("the white house") ||
            adr.Contains("1600 pennsylvania"),
            $"Expected White House address but got: {address.FormattedAddress}"
        );
        AssertWhiteHouseArea(address);
    }

    public static void AssertWhiteHouseArea(this Address address)
    {
        String adr = address.FormattedAddress.ToLowerInvariant();
        Assert.True(
            adr.Contains("washington") &&
            (adr.Contains("dc") || adr.Contains("district of columbia")),
            $"Expected Washington DC but got: {address.FormattedAddress}"
        );

        //just hoping that each geocoder implementation gets it somewhere near the vicinity
        Assert.InRange(address.Coordinates.Latitude, 38.85, 38.95);
        Assert.InRange(address.Coordinates.Longitude, -77.10, -76.95);
    }

    public static void AssertCanadianPrimeMinister(this Address address)
    {
        String adr = address.FormattedAddress.ToLowerInvariant();
        Assert.Contains("24 sussex", adr);
        Assert.Contains(" ottawa", adr);
        Assert.Contains(" on", adr);
        Assert.Contains("k1m", adr);
    }
}
