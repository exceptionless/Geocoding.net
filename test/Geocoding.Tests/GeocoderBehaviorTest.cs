using System.Globalization;
using Xunit;

namespace Geocoding.Tests;

public class GeocoderBehaviorTest : GeocoderTest
{
    private FakeGeocoder _fakeGeocoder;

    public GeocoderBehaviorTest()
        : base(new SettingsFixture()) { }

    protected override IGeocoder CreateGeocoder()
    {
        _fakeGeocoder = new FakeGeocoder();
        return _fakeGeocoder;
    }

    [Theory]
    [MemberData(nameof(CultureData), MemberType = typeof(GeocoderTest))]
    public override async Task CanGeocodeAddressUnderDifferentCultures(string cultureName)
    {
        await base.CanGeocodeAddressUnderDifferentCultures(cultureName);
        Assert.Equal(cultureName, _fakeGeocoder.LastCultureName);
    }

    [Theory]
    [MemberData(nameof(CultureData), MemberType = typeof(GeocoderTest))]
    public override async Task CanReverseGeocodeAddressUnderDifferentCultures(string cultureName)
    {
        await base.CanReverseGeocodeAddressUnderDifferentCultures(cultureName);
        Assert.Equal(cultureName, _fakeGeocoder.LastCultureName);
    }

    private sealed class FakeGeocoder : IGeocoder
    {
        public String LastCultureName { get; private set; }

        public Task<IEnumerable<Address>> GeocodeAsync(string address, CancellationToken cancellationToken = default)
        {
            LastCultureName = CultureInfo.CurrentCulture.Name;

            if (address.Contains("sdlkf"))
                return Task.FromResult(Enumerable.Empty<Address>());

            return Task.FromResult(CreateAddresses(address));
        }

        public Task<IEnumerable<Address>> GeocodeAsync(string street, string city, string state, string postalCode, string country, CancellationToken cancellationToken = default)
        {
            LastCultureName = CultureInfo.CurrentCulture.Name;
            return Task.FromResult<IEnumerable<Address>>(new[] { CreateWhiteHouseAddress() });
        }

        public Task<IEnumerable<Address>> ReverseGeocodeAsync(Location location, CancellationToken cancellationToken = default)
        {
            return ReverseGeocodeAsync(location.Latitude, location.Longitude, cancellationToken);
        }

        public Task<IEnumerable<Address>> ReverseGeocodeAsync(double latitude, double longitude, CancellationToken cancellationToken = default)
        {
            LastCultureName = CultureInfo.CurrentCulture.Name;
            return Task.FromResult<IEnumerable<Address>>(new[] { CreateWhiteHouseAddress() });
        }

        private static IEnumerable<Address> CreateAddresses(string address)
        {
            if (address.Contains("1600 pennsylvania", StringComparison.OrdinalIgnoreCase))
                return new[] { CreateWhiteHouseAddress() };

            if (address.Contains("24 sussex", StringComparison.OrdinalIgnoreCase))
                return new[] { new FakeAddress("24 Sussex Dr Ottawa ON K1M 1M4 Canada", new Location(45.44, -75.70)) };

            return new[] { new FakeAddress(address, new Location(38.90, -77.04)) };
        }

        private static FakeAddress CreateWhiteHouseAddress()
        {
            return new FakeAddress("1600 Pennsylvania Ave NW Washington DC 20500", new Location(38.8977, -77.0365));
        }
    }

    private sealed class FakeAddress : Address
    {
        public FakeAddress(string formattedAddress, Location coordinates)
            : base(formattedAddress, coordinates, "Fake") { }
    }
}
