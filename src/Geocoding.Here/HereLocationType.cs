﻿namespace Geocoding.Here
{
	/// <summary>
	/// Represents the location type returned by the HERE geocoding service.
	/// </summary>
    /// <remarks>
    /// https://developer.here.com/documentation/geocoder/topics/resource-type-response-geocode.html
    /// </remarks>
    public enum HereLocationType
	{
		/// <summary>The Unknown value.</summary>
		Unknown,
		/// <summary>The Point value.</summary>
		Point,
		/// <summary>The Area value.</summary>
		Area,
		/// <summary>The Address value.</summary>
		Address,
		/// <summary>The Trail value.</summary>
		Trail,
		/// <summary>The Park value.</summary>
		Park,
		/// <summary>The Lake value.</summary>
		Lake,
		/// <summary>The MountainPeak value.</summary>
		MountainPeak,
		/// <summary>The Volcano value.</summary>
		Volcano,
		/// <summary>The River value.</summary>
		River,
		/// <summary>The GolfCourse value.</summary>
		GolfCourse,
		/// <summary>The IndustrialComplex value.</summary>
		IndustrialComplex,
		/// <summary>The Island value.</summary>
		Island,
		/// <summary>The Woodland value.</summary>
		Woodland,
		/// <summary>The Cemetery value.</summary>
		Cemetery,
		/// <summary>The CanalWaterChannel value.</summary>
		CanalWaterChannel,
		/// <summary>The BayHarbor value.</summary>
		BayHarbor,
		/// <summary>The Airport value.</summary>
		Airport,
		/// <summary>The Hospital value.</summary>
		Hospital,
		/// <summary>The SportsComplex value.</summary>
		SportsComplex,
		/// <summary>The ShoppingCentre value.</summary>
		ShoppingCentre,
		/// <summary>The UniversityCollege value.</summary>
		UniversityCollege,
		/// <summary>The NativeAmericanReservation value.</summary>
		NativeAmericanReservation,
		/// <summary>The Railroad value.</summary>
		Railroad,
		/// <summary>The MilitaryBase value.</summary>
		MilitaryBase,
		/// <summary>The ParkingLot value.</summary>
		ParkingLot,
		/// <summary>The ParkingGarage value.</summary>
		ParkingGarage,
		/// <summary>The AnimalPark value.</summary>
		AnimalPark,
		/// <summary>The Beach value.</summary>
		Beach,
		/// <summary>The DistanceMarker value.</summary>
		DistanceMarker
	}
}
