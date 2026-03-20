﻿using System.Runtime.Serialization;

namespace Geocoding.Microsoft.Json
{
	/// <summary>
	/// Represents a Bing Maps address payload.
	/// </summary>
	[DataContract]
	public class Address
	{
		/// <summary>
		/// Gets or sets the street address line.
		/// </summary>
		[DataMember(Name = "addressLine")]
		public string AddressLine { get; set; }
		/// <summary>
		/// Gets or sets the primary administrative district.
		/// </summary>
		[DataMember(Name = "adminDistrict")]
		public string AdminDistrict { get; set; }
		/// <summary>
		/// Gets or sets the secondary administrative district.
		/// </summary>
		[DataMember(Name = "adminDistrict2")]
		public string AdminDistrict2 { get; set; }
		/// <summary>
		/// Gets or sets the country or region.
		/// </summary>
		[DataMember(Name = "countryRegion")]
		public string CountryRegion { get; set; }
		/// <summary>
		/// Gets or sets the formatted address.
		/// </summary>
		[DataMember(Name = "formattedAddress")]
		public string FormattedAddress { get; set; }
		/// <summary>
		/// Gets or sets the locality.
		/// </summary>
		[DataMember(Name = "locality")]
		public string Locality { get; set; }
		/// <summary>
		/// Gets or sets the neighborhood.
		/// </summary>
		[DataMember(Name = "neighborhood")]
		public string Neighborhood { get; set; }
		/// <summary>
		/// Gets or sets the postal code.
		/// </summary>
		[DataMember(Name = "postalCode")]
		public string PostalCode { get; set; }
	}
	/// <summary>
	/// Represents a Bing Maps bounding box.
	/// </summary>
	[DataContract]
	public class BoundingBox
	{
		/// <summary>
		/// Gets or sets the southern latitude.
		/// </summary>
		[DataMember(Name = "southLatitude")]
		public double SouthLatitude { get; set; }
		/// <summary>
		/// Gets or sets the western longitude.
		/// </summary>
		[DataMember(Name = "westLongitude")]
		public double WestLongitude { get; set; }
		/// <summary>
		/// Gets or sets the northern latitude.
		/// </summary>
		[DataMember(Name = "northLatitude")]
		public double NorthLatitude { get; set; }
		/// <summary>
		/// Gets or sets the eastern longitude.
		/// </summary>
		[DataMember(Name = "eastLongitude")]
		public double EastLongitude { get; set; }
	}
	/// <summary>
	/// Represents a Bing Maps hint value.
	/// </summary>
	[DataContract]
	public class Hint
	{
		/// <summary>
		/// Gets or sets the hint type.
		/// </summary>
		[DataMember(Name = "hintType")]
		public string HintType { get; set; }
		/// <summary>
		/// Gets or sets the hint value.
		/// </summary>
		[DataMember(Name = "value")]
		public string Value { get; set; }
	}
	/// <summary>
	/// Represents a Bing Maps maneuver instruction.
	/// </summary>
	[DataContract]
	public class Instruction
	{
		/// <summary>
		/// Gets or sets the maneuver type.
		/// </summary>
		[DataMember(Name = "maneuverType")]
		public string ManeuverType { get; set; }
		/// <summary>
		/// Gets or sets the instruction text.
		/// </summary>
		[DataMember(Name = "text")]
		public string Text { get; set; }
		//[DataMember(Name = "value")]
		//public string Value { get; set; }
	}
	/// <summary>
	/// Represents a Bing Maps itinerary item.
	/// </summary>
	[DataContract]
	public class ItineraryItem
	{
		/// <summary>
		/// Gets or sets the travel mode.
		/// </summary>
		[DataMember(Name = "travelMode")]
		public string TravelMode { get; set; }
		/// <summary>
		/// Gets or sets the travel distance.
		/// </summary>
		[DataMember(Name = "travelDistance")]
		public double TravelDistance { get; set; }
		/// <summary>
		/// Gets or sets the travel duration.
		/// </summary>
		[DataMember(Name = "travelDuration")]
		public long TravelDuration { get; set; }
		/// <summary>
		/// Gets or sets the maneuver point.
		/// </summary>
		[DataMember(Name = "maneuverPoint")]
		public Point ManeuverPoint { get; set; }
		/// <summary>
		/// Gets or sets the instruction.
		/// </summary>
		[DataMember(Name = "instruction")]
		public Instruction Instruction { get; set; }
		/// <summary>
		/// Gets or sets the compass direction.
		/// </summary>
		[DataMember(Name = "compassDirection")]
		public string CompassDirection { get; set; }
		/// <summary>
		/// Gets or sets the hints.
		/// </summary>
		[DataMember(Name = "hint")]
		public Hint[] Hint { get; set; }
		/// <summary>
		/// Gets or sets the warnings.
		/// </summary>
		[DataMember(Name = "warning")]
		public Warning[] Warning { get; set; }
	}
	/// <summary>
	/// Represents a Bing Maps polyline.
	/// </summary>
	[DataContract]
	public class Line
	{
		/// <summary>
		/// Gets or sets the points in the line.
		/// </summary>
		[DataMember(Name = "point")]
		public Point[] Point { get; set; }
	}
	/// <summary>
	/// Represents a Bing Maps resource link.
	/// </summary>
	[DataContract]
	public class Link
	{
		/// <summary>
		/// Gets or sets the link role.
		/// </summary>
		[DataMember(Name = "role")]
		public string Role { get; set; }
		/// <summary>
		/// Gets or sets the link name.
		/// </summary>
		[DataMember(Name = "name")]
		public string Name { get; set; }
		/// <summary>
		/// Gets or sets the link value.
		/// </summary>
		[DataMember(Name = "value")]
		public string Value { get; set; }
	}
	/// <summary>
	/// Represents a Bing Maps location resource.
	/// </summary>
	[DataContract(Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
	public class Location : Resource
	{
		/// <summary>
		/// Gets or sets the entity type.
		/// </summary>
		[DataMember(Name = "entityType")]
		public string EntityType { get; set; }
		/// <summary>
		/// Gets or sets the structured address.
		/// </summary>
		[DataMember(Name = "address")]
		public Address Address { get; set; }
		/// <summary>
		/// Gets or sets the confidence level.
		/// </summary>
		[DataMember(Name = "confidence")]
		public string Confidence { get; set; }
	}
	/// <summary>
	/// Represents a Bing Maps point shape.
	/// </summary>
	[DataContract]
	public class Point : Shape
	{
		/// <summary>
		/// Gets or sets the latitude/longitude coordinates.
		/// </summary>
		[DataMember(Name = "coordinates")]
		public double[] Coordinates { get; set; }
		//[DataMember(Name = "latitude")]
		//public double Latitude { get; set; }
		//[DataMember(Name = "longitude")]
		//public double Longitude { get; set; }
	}
	/// <summary>
	/// Represents a Bing Maps resource.
	/// </summary>
	[DataContract]
	[KnownType(typeof(Location))]
	[KnownType(typeof(Route))]
	public class Resource
	{
		/// <summary>
		/// Gets or sets the resource name.
		/// </summary>
		[DataMember(Name = "name")]
		public string Name { get; set; }
		/// <summary>
		/// Gets or sets the resource identifier.
		/// </summary>
		[DataMember(Name = "id")]
		public string Id { get; set; }
		/// <summary>
		/// Gets or sets the resource links.
		/// </summary>
		[DataMember(Name = "link")]
		public Link[] Link { get; set; }
		/// <summary>
		/// Gets or sets the representative point.
		/// </summary>
		[DataMember(Name = "point")]
		public Point Point { get; set; }
		/// <summary>
		/// Gets or sets the bounding box.
		/// </summary>
		[DataMember(Name = "boundingBox")]
		public BoundingBox BoundingBox { get; set; }
	}
	/// <summary>
	/// Represents a Bing Maps resource set.
	/// </summary>
	[DataContract]
	public class ResourceSet
	{
		/// <summary>
		/// Gets or sets the estimated total resource count.
		/// </summary>
		[DataMember(Name = "estimatedTotal")]
		public long EstimatedTotal { get; set; }
		/// <summary>
		/// Gets or sets the resources.
		/// </summary>
		[DataMember(Name = "resources")]
		public Resource[] Resources { get; set; }
	}
	/// <summary>
	/// Represents the top-level Bing Maps response.
	/// </summary>
	[DataContract]
	public class Response
	{
		/// <summary>
		/// Gets or sets the copyright text.
		/// </summary>
		[DataMember(Name = "copyright")]
		public string Copyright { get; set; }
		/// <summary>
		/// Gets or sets the brand logo URI.
		/// </summary>
		[DataMember(Name = "brandLogoUri")]
		public string BrandLogoUri { get; set; }
		/// <summary>
		/// Gets or sets the HTTP-like status code.
		/// </summary>
		[DataMember(Name = "statusCode")]
		public int StatusCode { get; set; }
		/// <summary>
		/// Gets or sets the status description.
		/// </summary>
		[DataMember(Name = "statusDescription")]
		public string StatusDescription { get; set; }
		/// <summary>
		/// Gets or sets the authentication result code.
		/// </summary>
		[DataMember(Name = "authenticationResultCode")]
		public string AuthenticationResultCode { get; set; }
		/// <summary>
		/// Gets or sets the error details.
		/// </summary>
		[DataMember(Name = "errorDetails")]
		public string[] errorDetails { get; set; }
		/// <summary>
		/// Gets or sets the trace identifier.
		/// </summary>
		[DataMember(Name = "traceId")]
		public string TraceId { get; set; }
		/// <summary>
		/// Gets or sets the resource sets.
		/// </summary>
		[DataMember(Name = "resourceSets")]
		public ResourceSet[] ResourceSets { get; set; }
	}
	/// <summary>
	/// Represents a Bing Maps route resource.
	/// </summary>
	[DataContract(Namespace = "http://schemas.microsoft.com/search/local/ws/rest/v1")]
	public class Route : Resource
	{
		/// <summary>
		/// Gets or sets the distance unit.
		/// </summary>
		[DataMember(Name = "distanceUnit")]
		public string DistanceUnit { get; set; }
		/// <summary>
		/// Gets or sets the duration unit.
		/// </summary>
		[DataMember(Name = "durationUnit")]
		public string DurationUnit { get; set; }
		/// <summary>
		/// Gets or sets the travel distance.
		/// </summary>
		[DataMember(Name = "travelDistance")]
		public double TravelDistance { get; set; }
		/// <summary>
		/// Gets or sets the travel duration.
		/// </summary>
		[DataMember(Name = "travelDuration")]
		public long TravelDuration { get; set; }
		/// <summary>
		/// Gets or sets the route legs.
		/// </summary>
		[DataMember(Name = "routeLegs")]
		public RouteLeg[] RouteLegs { get; set; }
		/// <summary>
		/// Gets or sets the route path.
		/// </summary>
		[DataMember(Name = "routePath")]
		public RoutePath RoutePath { get; set; }
	}
	/// <summary>
	/// Represents a Bing Maps route leg.
	/// </summary>
	[DataContract]
	public class RouteLeg
	{
		/// <summary>
		/// Gets or sets the travel distance.
		/// </summary>
		[DataMember(Name = "travelDistance")]
		public double TravelDistance { get; set; }
		/// <summary>
		/// Gets or sets the travel duration.
		/// </summary>
		[DataMember(Name = "travelDuration")]
		public long TravelDuration { get; set; }
		/// <summary>
		/// Gets or sets the actual start point.
		/// </summary>
		[DataMember(Name = "actualStart")]
		public Point ActualStart { get; set; }
		/// <summary>
		/// Gets or sets the actual end point.
		/// </summary>
		[DataMember(Name = "actualEnd")]
		public Point ActualEnd { get; set; }
		/// <summary>
		/// Gets or sets the start location.
		/// </summary>
		[DataMember(Name = "startLocation")]
		public Location StartLocation { get; set; }
		/// <summary>
		/// Gets or sets the end location.
		/// </summary>
		[DataMember(Name = "endLocation")]
		public Location EndLocation { get; set; }
		/// <summary>
		/// Gets or sets the itinerary items.
		/// </summary>
		[DataMember(Name = "itineraryItems")]
		public ItineraryItem[] ItineraryItems { get; set; }
	}
	/// <summary>
	/// Represents a Bing Maps route path.
	/// </summary>
	[DataContract]
	public class RoutePath
	{
		/// <summary>
		/// Gets or sets the route line.
		/// </summary>
		[DataMember(Name = "line")]
		public Line Line { get; set; }
	}
	/// <summary>
	/// Represents a Bing Maps shape.
	/// </summary>
	[DataContract]
	[KnownType(typeof(Point))]
	public class Shape
	{
		/// <summary>
		/// Gets or sets the bounding box coordinates.
		/// </summary>
		[DataMember(Name = "boundingBox")]
		public double[] BoundingBox { get; set; }
	}
	/// <summary>
	/// Represents a Bing Maps warning.
	/// </summary>
	[DataContract]
	public class Warning
	{
		/// <summary>
		/// Gets or sets the warning type.
		/// </summary>
		[DataMember(Name = "warningType")]
		public string WarningType { get; set; }
		/// <summary>
		/// Gets or sets the warning severity.
		/// </summary>
		[DataMember(Name = "severity")]
		public string Severity { get; set; }
		/// <summary>
		/// Gets or sets the warning value.
		/// </summary>
		[DataMember(Name = "value")]
		public string Value { get; set; }
	}
}