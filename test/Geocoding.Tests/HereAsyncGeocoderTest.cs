﻿using Geocoding.Here;
using Xunit;

namespace Geocoding.Tests
{
	[Collection("Settings")]
	public class HereAsyncGeocoderTest : AsyncGeocoderTest
	{
		public HereAsyncGeocoderTest(SettingsFixture settings)
			: base(settings) { }

		protected override IGeocoder CreateAsyncGeocoder()
		{
			SettingsFixture.SkipIfMissing(_settings.HereAppId, nameof(SettingsFixture.HereAppId));
			SettingsFixture.SkipIfMissing(_settings.HereAppCode, nameof(SettingsFixture.HereAppCode));
			return new HereGeocoder(_settings.HereAppId, _settings.HereAppCode);
		}
	}
}
