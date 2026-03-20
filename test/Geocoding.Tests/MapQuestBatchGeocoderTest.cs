﻿using Geocoding.MapQuest;
using Xunit;

namespace Geocoding.Tests
{
	[Collection("Settings")]
	public class MapQuestBatchGeocoderTest : BatchGeocoderTest
	{
		public MapQuestBatchGeocoderTest(SettingsFixture settings)
			: base(settings) { }

		protected override IBatchGeocoder CreateBatchGeocoder()
		{
			SettingsFixture.SkipIfMissing(_settings.MapQuestKey, nameof(SettingsFixture.MapQuestKey));
			return new MapQuestGeocoder(_settings.MapQuestKey);
		}
	}
}
